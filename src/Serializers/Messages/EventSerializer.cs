using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlStreamStore.Demo.Serializers.Messages
{
    public interface IEventSerializer
    {
        string SerializeEvent(string type, object @event);
        object DeserializeEvent(string type, string json);
    }

    public class EventSerializer : IEventSerializer
    {
        private readonly IDictionary<string, IMessageSerializer> _messageSerializers;

        public EventSerializer(IEnumerable<IMessageSerializer> messageSerializers)
        {
            _messageSerializers = messageSerializers.ToDictionary(x => x.Type, x => x);
        }

        public string SerializeEvent(string type, object @event)
        {
            if (_messageSerializers.TryGetValue(type, out IMessageSerializer serializer))
            {
                return serializer.Serialize(@event);
            }
            throw new InvalidOperationException($"no serializer defined for event {type}");
        }

        public object DeserializeEvent(string type, string json)
        {
            if (_messageSerializers.TryGetValue(type, out IMessageSerializer serializer))
            {
                return serializer.Deserialize(json);
            }
            throw new InvalidOperationException($"no serializer defined for event {type}");
        }
    }
}