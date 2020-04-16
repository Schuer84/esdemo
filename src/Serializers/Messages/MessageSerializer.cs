using System;

namespace SqlStreamStore.Demo.Serializers.Messages
{
    public abstract class MessageSerializer : IMessageSerializer
    {
        protected MessageSerializer(string type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Type { get; }

        public abstract object Deserialize(string obj);
        public abstract string Serialize(object obj);
    }
}