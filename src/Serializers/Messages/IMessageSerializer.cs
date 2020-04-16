namespace SqlStreamStore.Demo
{
    public interface IMessageSerializer
    {
        string Type { get; }

        object Deserialize(string obj);
        string Serialize(object obj);
    }
}