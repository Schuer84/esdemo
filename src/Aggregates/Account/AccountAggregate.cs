using System;
using SqlStreamStore.Demo.Events;
using SqlStreamStore.Demo.Events.Account;

namespace SqlStreamStore.Demo.Aggregates.Account
{
    public class AccountAggregate : AggregateBase<AccountAggregateState>
    {
        public AccountAggregate(IEventStore eventStore)
            : base(eventStore)
        {
        }

        public void Withdraw(decimal amount)
        {
            if (State.CanWithdraw(amount))
            {
                var amountWithdrawn = new AmountWithdrawn()
                {
                    Amount = amount,
                    AccountId = GetAccountId()
                };
                
                Emit(amountWithdrawn);
            }
            else
            {
                throw new InvalidOperationException($"unable to withdraw {amount}");
            }
        }

        public void Deposit(decimal amount)
        {
            if (State.CanDeposit(amount))
            {
                var amountDeposited = new AmountDeposited()
                {
                    Amount = amount,
                    AccountId = GetAccountId()
                };
                Emit(amountDeposited);
            }
            else
            {
                throw new InvalidOperationException($"unable to deposit {amount}");
            }
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(Id.Replace("Account:", ""));
        }
        
}
}