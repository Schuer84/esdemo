using System;

namespace SqlStreamStore.Demo
{
    public abstract class JsonMessageSerializer<T> : MessageSerializer
    {
        private readonly IJsonSerializer _jsonSerializer;

        protected JsonMessageSerializer(string type, IJsonSerializer jsonSerializer) : base(type)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public override object Deserialize(string obj)
        {
            return _jsonSerializer.Deserialize<T>(obj);
        }

        public override string Serialize(object obj)
        {
            return _jsonSerializer.Serialize(obj);
        }
    }
}