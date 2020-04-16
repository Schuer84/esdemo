using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo
{
    public class EventStore : IEventStore
    {
        private readonly IStreamStore _streamStore;
        private readonly IDictionary<string, IMessageSerializer> _messageSerializers;

        public EventStore(IStreamStore streamStore, IEnumerable<IMessageSerializer> messageSerializers)
        {
            if (messageSerializers == null || !messageSerializers.Any())
            {
                throw new ArgumentException("no message serializers provided");
            }

            _streamStore = streamStore ?? throw new ArgumentNullException(nameof(streamStore));
            _messageSerializers = messageSerializers.ToDictionary(x => x.Type, x => x);
        }

        public async Task<IEnumerable<Event>> GetAllAsync(string streamId, CancellationToken cancellationToken)
        {
            var events = new List<Event>();
            var readAllPage = await _streamStore.ReadStreamForwards(streamId, 0, 100, cancellationToken);
            while (!readAllPage.IsEnd)
            {
                foreach (var message in readAllPage.Messages)
                {
                    if (_messageSerializers.TryGetValue(message.Type, out IMessageSerializer serializer))
                    {
                        var json = await message.GetJsonData(cancellationToken);
                        var data = (Event)serializer.Deserialize(json);
                        //message.StreamVersion == data.ExpectedVersion?

                        events.Add(data);
                    }
                }
                readAllPage = await readAllPage.ReadNext(cancellationToken);
            }

            return events;
        }

        public async Task AppendAsync(string streamId, Event @event, CancellationToken cancellationToken)
        {
            if (_messageSerializers.TryGetValue(@event.Type, out IMessageSerializer serializer))
            {
                var json = serializer.Serialize(@event);
                var message = new NewStreamMessage(Guid.NewGuid(), @event.Type, json);
                await _streamStore.AppendToStream(streamId, @event.ExpectedVersion, message, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"no serializer defined for event {@event.Type}");
            }
        }
    }
}