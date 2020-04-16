using System.Collections;

namespace SqlStreamStore.Demo.Aggregates
{
    public interface IAggregateState
    {
    }

    public interface IChangeSet
    {
        int Version { get; }
        IEnumerable Events { get; }
    }

    public class ChangeSet : IChangeSet {
        public int Version { get; set; }
        public IEnumerable Events { get; set; }
    }
}