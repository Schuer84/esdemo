using System;
using System.Collections.Generic;
using System.Text;
using SqlStreamStore.Demo.Aggregates;
using SqlStreamStore.Demo.Aggregates.Account;

namespace SqlStreamStore.Demo.Commands.Account
{
    public class DepositAmountCommand : AccountCommand
    {
        public decimal Amount { get; set; }
    }

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
