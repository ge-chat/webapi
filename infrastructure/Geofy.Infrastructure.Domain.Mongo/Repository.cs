using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Geofy.Infrastructure.Domain.Interfaces;
using Geofy.Infrastructure.Domain.Transitions;
using Geofy.Infrastructure.Domain.Transitions.Interfaces;
using Geofy.Infrastructure.Domain.Utilities;
using Geofy.Infrastructure.ServiceBus.Dispatching;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using MongoDB.Bson;
using System.Linq;

namespace Geofy.Infrastructure.Domain.Mongo
{
    public class Repository : IRepository
    {
        private readonly ITransitionRepository _transitionStorage;
        private readonly IEventBus _eventBus;

        public Repository(ITransitionRepository transitionStorage, IEventBus eventBus)
        {
            _transitionStorage = transitionStorage;
            _eventBus = eventBus;
        }

        public async Task Save(string aggregateId, Aggregate aggregate)
        {
            if (aggregateId == null)
                throw new ArgumentException(
                    $"Aggregate ID is null when trying to save {aggregate.GetType().FullName} aggregate. Please specify aggregate ID.");

            if (string.IsNullOrEmpty(aggregateId))
                throw new ArgumentException(
                    $"Aggregate ID is empty string when trying to save {aggregate.GetType().FullName} aggregate. Please specify aggregate ID.");

            var transition = CreateTransition(aggregateId, aggregate);
            await _transitionStorage.AppendTransition(transition);
            await _eventBus.PublishAsync(transition.Events.Select(e => (IEvent)e.Data).ToList());
        }

        public async Task<TAggregate> GetById<TAggregate>(String id)
            where TAggregate : Aggregate
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException(
                    $"Aggregate ID was not specified when trying to get by id {typeof (TAggregate).FullName} aggregate");

            var transitions = await _transitionStorage.GetTransitions(id, 0, int.MaxValue);

            var aggregate = AggregateCreator.CreateAggregateRoot<TAggregate>();
            var state = AggregateCreator.CreateAggregateState(typeof(TAggregate));
            StateSpooler.Spool((AggregateState)state, transitions);
            aggregate.Setup(state, transitions.Count == 0 ? 0 : transitions.Last().Id.Version);

            return aggregate;
        }


        /// <summary>
        /// Perform action on aggregate with specified id.
        /// Aggregate should be already created.
        /// </summary>
        public async Task Perform<TAggregate>(String id, Action<TAggregate> action)
            where TAggregate : Aggregate
        {
            var aggregate = await GetById<TAggregate>(id);
            action(aggregate);
            await Save(id, aggregate);
        }

        /// <summary>
        /// Create changeset. Used to persist changes in aggregate
        /// </summary>
        /// <returns></returns>
        public Transition CreateTransition(String id, Aggregate aggregate)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception(
                    $"ID was not specified for domain object. AggregateRoot [{this.GetType().FullName}] doesn't have correct ID. Maybe you forgot to set an _id field?");

            var currentTime = DateTime.UtcNow;
            var transitionEvents = new List<TransitionEvent>();
            foreach (var e in aggregate.Changes)
            {
                e.Metadata.EventId = ObjectId.GenerateNewId().ToString();
                e.Metadata.StoredDate = currentTime;
                e.Metadata.TypeName = e.GetType().Name;

                // Take some metadata properties from command
                var command = Dispatcher.CurrentMessage as ICommand;
                if (command?.Metadata != null)
                {
                    e.Metadata.CommandId = command.Metadata.CommandId;
                    e.Metadata.UserId = command.Metadata.UserId;
                }

                transitionEvents.Add(new TransitionEvent(e.GetType().AssemblyQualifiedName, e));
            }

            return new Transition(
                new TransitionId(id, aggregate.Version + 1),
                aggregate.GetType().AssemblyQualifiedName,
                currentTime,
                transitionEvents);
        }

    }

    public class Repository<TAggregate> : Repository, IRepository<TAggregate> where TAggregate : Aggregate
    {
        public Repository(ITransitionRepository transitionStorage, IEventBus eventBus)
            : base(transitionStorage, eventBus)
        {
        }

        public async Task Save(String aggregateId, TAggregate aggregate)
        {
            await base.Save(aggregateId, aggregate);
        }

        public Task<TAggregate> GetById(String id)
        {
            return GetById<TAggregate>(id);
        }

        public Task Perform(string id, Action<TAggregate> action)
        {
            return Perform<TAggregate>(id, action);
        }
    }
}
