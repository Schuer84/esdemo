using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo.Events
{
    public interface IEventStore
    {
        Task<IEnumerable<Event>> GetAllAsync(StreamId streamId, CancellationToken cancellationToken);
        Task AppendAsync(StreamId streamId, Event @event, CancellationToken cancellationToken);
    }
}