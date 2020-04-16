namespace SqlStreamStore.Demo.Serializers.Messages
{
    public interface IMessageSerializer
    {
        string Type { get; }

        object Deserialize(string obj);
        string Serialize(object obj);
    }
}