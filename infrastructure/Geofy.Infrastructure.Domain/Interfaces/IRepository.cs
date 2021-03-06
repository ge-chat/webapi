using System;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.Domain.Interfaces
{
    public interface IRepository 
    {
        Task Save(String aggregateId, Aggregate aggregate, ICommandMetadata commandMetadata);

        /// <summary>
        /// Generic version
        /// </summary>
        Task<TAggregate> GetById<TAggregate>(String id)
            where TAggregate : Aggregate;

        /// <summary>
        /// Perform action on aggregate with specified id.
        /// Aggregate should be already created.
        /// </summary>
        Task Perform<TAggregate>(String id, Action<TAggregate> action, ICommandMetadata commandMetadata)
            where TAggregate : Aggregate;
    }

    public interface IRepository<TAggregate> where TAggregate : Aggregate
    {
        Task Save(String aggregateId, TAggregate aggregate, ICommandMetadata commandMetadata);

        /// <summary>
        /// Generic version
        /// </summary>
        Task<TAggregate> GetById(String id);

        /// <summary>
        /// Perform action on aggregate with specified id.
        /// Aggregate should be already created.
        /// </summary>
        Task Perform(String id, Action<TAggregate> action, ICommandMetadata commandMetadata);
    }
}