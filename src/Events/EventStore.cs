using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Serializers.Messages;
using SqlStreamStore.Streams;

namespace SqlStreamStore.Demo.Events
{

    

    public class EventStore : IEventStore
    {
        private readonly IStreamStore _streamStore;
        private readonly IEventSerializer _eventSerializer;

        public EventStore(IStreamStore streamStore, IEventSerializer eventSerializer)
        {
            _streamStore = streamStore ?? throw new ArgumentNullException(nameof(streamStore));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
        }

        public async Task<IEnumerable<Event>> GetAllAsync(StreamId streamId, CancellationToken cancellationToken)
        {
            var events = new List<Event>();
            var loaded = false;
            var readAllPage = await _streamStore.ReadStreamForwards(streamId, 0, 100, cancellationToken);
            while (!loaded)
            {
                foreach (var message in readAllPage.Messages)
                {   
                    var json = await message.GetJsonData(cancellationToken);
                    var data = (Event)_eventSerializer.DeserializeEvent(message.Type, json);
                        data.ExpectedVersion = message.StreamVersion;
                    events.Add(data);
                }

                if (!readAllPage.IsEnd)
                {
                    readAllPage = await readAllPage.ReadNext(cancellationToken);
                }
                else
                {
                    loaded = true;
                }
            }

            return events;
        }

        public async Task AppendAsync(StreamId streamId, Event @event, CancellationToken cancellationToken)
        {
            var json = _eventSerializer.SerializeEvent(@event.Type, @event);
            var message = new NewStreamMessage(Guid.NewGuid(), @event.Type, json);
            await _streamStore.AppendToStream(streamId, @event.ExpectedVersion, message, cancellationToken);
        }
    }
}