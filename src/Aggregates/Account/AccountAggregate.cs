using System;
using SqlStreamStore.Demo.Events;
using SqlStreamStore.Demo.Events.Account;

namespace SqlStreamStore.Demo.Aggregates.Account
{
    public class AccountAggregate : AggregateBase<AccountAggregateState>
    {
        public AccountAggregate(IEventStore eventStore) 
            : base(eventStore)
        { }

        public void Withdraw(decimal amount)
        {
            if (State.CanWithdraw(amount))
            {
                var withdrawEvent = new AmountWithdrawn(Guid.NewGuid(), amount, DateTime.UtcNow);
                Emit(withdrawEvent);

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
                var deposited = new AmountDeposited(Guid.NewGuid(), amount, DateTime.UtcNow);
                Emit(deposited);
            }
            else
            {
                throw new InvalidOperationException($"unable to deposit {amount}");
            }
        }


    }
}