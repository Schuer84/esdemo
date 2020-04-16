using System.Threading.Tasks;

namespace SqlStreamStore.Demo.Commands
{
    public interface ICommandHandler
    {
        Task Handle(ICommand command);

        Task Handle<TCommand>(TCommand command) 
            where TCommand: ICommand;
    }

    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }

    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public abstract Task Handle(TCommand command);

    }
}