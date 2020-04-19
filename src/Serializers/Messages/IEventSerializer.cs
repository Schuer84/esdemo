namespace SqlStreamStore.Demo.Serializers.Messages
{
    public interface IEventSerializer
    {
        string SerializeEvent(string type, object @event);
        object DeserializeEvent(string type, string json);
    }
}