using System;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Aggregates;

namespace SqlStreamStore.Demo.Commands
{
    public interface ICommandHandler
    {
        Task Handle(ICommand command, CancellationToken cancellationToken);

        Task Handle<TCommand>(TCommand command, CancellationToken cancellationToken) 
            where TCommand: ICommand;
    }

    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task Handle(TCommand command, CancellationToken cancellationToken);
    }

    public abstract class CommandHandler<TCommand, TAggregate> : ICommandHandler<TCommand>
        where TCommand : ICommand
        where TAggregate: IEventSourcedAggregate
    {
        private readonly IAggregateRepository _aggregateRepository;

        protected CommandHandler(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
        }

        public virtual async Task Handle(TCommand command, CancellationToken cancellationToken)
        {
            var aggregateId = command.GetAggregateId();
            var aggregate = await _aggregateRepository.GetById<TAggregate>(aggregateId, cancellationToken);
            HandleCommand(aggregate, command);
            await _aggregateRepository.Save(aggregate, cancellationToken);
        }

        protected abstract void HandleCommand(TAggregate aggregate, TCommand command);


    }
}