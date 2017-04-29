using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geofy.Infrastructure.Domain.Transitions.Interfaces;

namespace Geofy.Infrastructure.Domain.Transitions.InMemory
{
    public class InMemoryTransitionRepository : ITransitionRepository
    {
        private readonly List<Transition> _transitions = new List<Transition>();

        public Task AppendTransition(Transition transition)
        {
            _transitions.Add(transition);
            return Task.CompletedTask;
        }

        public Task AppendTransitions(IEnumerable<Transition> transitions)
        {
            _transitions.AddRange(transitions);
            return Task.CompletedTask;
        }

        public Task<List<Transition>> GetTransitions(string streamId, int fromVersion, int toVersion)
        {
            return Task.FromResult(_transitions.Where(t =>
                t.Id.StreamId == streamId &&
                t.Id.Version >= fromVersion &&
                t.Id.Version <= toVersion)
                .ToList());
        }

        public Task<IEnumerable<Transition>> GetTransitions(int startIndex, int count)
        {
            return Task.FromResult(_transitions.Skip(startIndex).Take(count));
        }

        public Task<long> CountTransitions()
        {
            return Task.FromResult((long)_transitions.Count);
        }

        /// <summary>
        /// Get all transitions ordered ascendantly by Timestamp of transiton
        /// Should be used only for testing and for very simple event replying 
        /// </summary>
        public Task<IEnumerable<Transition>> GetTransitions()
        {
            return Task.FromResult(_transitions.AsEnumerable());
        }

        public Task RemoveTransition(string streamId, int version)
        {
            _transitions.RemoveAll(t => t.Id.StreamId == streamId && t.Id.Version == version);
            return Task.CompletedTask;
        }

        public Task RemoveStream(string streamId)
        {
            _transitions.RemoveAll(t => t.Id.StreamId == streamId);
            return Task.CompletedTask;
        }

        public Task CreateIndexes()
        {
            // Nothing to do here. In Memory Repository does not need indexes.
            return Task.CompletedTask;
        }
    }
}