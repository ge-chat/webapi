using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geofy.Infrastructure.Domain.Transitions.Interfaces;

namespace Geofy.Infrastructure.Domain.Transitions.InMemory
{
    public class InMemoryTransitionRepository : ITransitionRepository
    {
        private readonly List<Transition> _transitions = new List<Transition>();

        public async Task AppendTransition(Transition transition)
        {
            _transitions.Add(transition);
        }

        public async Task AppendTransitions(IEnumerable<Transition> transitions)
        {
            _transitions.AddRange(transitions);
        }

        public async Task<List<Transition>> GetTransitions(string streamId, int fromVersion, int toVersion)
        {
            return _transitions.Where(t =>
                t.Id.StreamId == streamId &&
                t.Id.Version >= fromVersion &&
                t.Id.Version <= toVersion)
                .ToList();
        }

        public async Task<IEnumerable<Transition>> GetTransitions(int startIndex, int count)
        {
            return _transitions.Skip(startIndex).Take(count);
        }

        public async Task<long> CountTransitions()
        {
            return _transitions.Count;
        }

        /// <summary>
        /// Get all transitions ordered ascendantly by Timestamp of transiton
        /// Should be used only for testing and for very simple event replying 
        /// </summary>
        public async Task<IEnumerable<Transition>> GetTransitions()
        {
            return _transitions;
        }

        public async Task RemoveTransition(string streamId, int version)
        {
            _transitions.RemoveAll(t => t.Id.StreamId == streamId && t.Id.Version == version);
        }

        public async Task RemoveStream(string streamId)
        {
            _transitions.RemoveAll(t => t.Id.StreamId == streamId);
        }

        public async Task CreateIndexes()
        {
            // Nothing to do here. In Memory Repository does not need indexes.
        }
    }
}