namespace SqlStreamStore.Demo.Commands
{
    public interface ICommand
    {
        string GetAggregateId();
    }

}