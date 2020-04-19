using SqlStreamStore.Demo.Aggregates;
using SqlStreamStore.Demo.Aggregates.Account;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class WithdrawAmountCommandHandler : CommandHandler<WithdrawAmountCommand, AccountAggregate>
    {
        public WithdrawAmountCommandHandler(IAggregateRepository aggregateRepository) 
            : base(aggregateRepository)
        {
        }

        protected override void HandleCommand(AccountAggregate aggregate, WithdrawAmountCommand command)
        {
            aggregate.Withdraw(command.Amount);
        }
    }
}