using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SqlStreamStore.Demo.Events;

namespace SqlStreamStore.Demo.Aggregates
{
    public interface IAggregateState
    {
    }

    public interface IChangeSet
    {
        int Version { get; }
        IEnumerable<Event> Events { get; }
        bool HasChanges { get; }
    }

    public class ChangeSet : IChangeSet {

        public int Version { get; set; }
        public IEnumerable<Event> Events { get; set; }

        public bool HasChanges => Events.Any();
    }
}