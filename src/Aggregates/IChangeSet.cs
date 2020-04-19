using System.Collections.Generic;
using SqlStreamStore.Demo.Events;

namespace SqlStreamStore.Demo.Aggregates
{
    public interface IChangeSet
    {
        int Version { get; }
        IEnumerable<Event> Events { get; }
        bool HasChanges { get; }
    }
}