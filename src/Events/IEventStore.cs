using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlStreamStore.Demo
{
    public interface IEventStore
    {
        Task<IEnumerable<Event>> GetAllAsync(string streamId, CancellationToken cancellationToken);
        Task AppendAsync(string streamId, Event @event, CancellationToken cancellationToken);
    }
}