using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.Domain.Transitions.Interfaces
{
    public interface ITransitionRepository
    {
        /// <summary>
        /// Append transition to store
        /// </summary>
        Task AppendTransition(Transition transition);

        Task AppendTransitions(IEnumerable<Transition> transitions);

        /// <summary>
        /// Get transitions for specified streamId (aggregate ID).
        /// Will return empty list, if such stream not exists.
        /// </summary>
        Task<List<Transition>> GetTransitions(String streamId, Int32 fromVersion, Int32 toVersion);

        /// <summary>
        /// Get all transitions ordered ascendantly by Timestamp
        /// </summary>
        Task<IEnumerable<Transition>> GetTransitions();

        /// <summary>
        /// Get transitions paged and ordered ascendantly by Timestamp
        /// </summary>
        Task<IEnumerable<Transition>> GetTransitions(Int32 startIndex, Int32 count);

        /// <summary>
        /// Remove single transition from stream
        /// </summary>
        Task RemoveTransition(String streamId, Int32 version);

        /// <summary>
        /// Remove stream (and all transitions belonging to this stream)
        /// </summary>
        /// <param name="streamId"></param>
        Task RemoveStream(String streamId);

        /// <summary>
        /// Returns total number of transitions in store
        /// </summary>
        /// <returns></returns>
        Task<long> CountTransitions();

        /// <summary>
        /// Build indexes for transitions
        /// </summary>
        Task CreateIndexes();
    }
}
