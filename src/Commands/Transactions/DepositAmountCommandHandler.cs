using SqlStreamStore.Demo.Aggregates;
using SqlStreamStore.Demo.Aggregates.Account;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class DepositAmountCommandHandler : CommandHandler<DepositAmountCommand, AccountAggregate>
    {
        public DepositAmountCommandHandler(IAggregateRepository aggregateRepository)
            : base(aggregateRepository)
        {
        }

        protected override void HandleCommand(AccountAggregate aggregate, DepositAmountCommand command)
        {
            aggregate.Deposit(command.Amount);
        }
    }
}