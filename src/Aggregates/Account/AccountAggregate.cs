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

        public Guid Withdraw(decimal amount)
        {
            if (State.CanWithdraw(amount))
            {
                var withdrawEvent = new AmountWithdrawn(Guid.NewGuid(), amount, DateTime.UtcNow);
                Emit(withdrawEvent);
                return withdrawEvent.TransactionId;
            }
            throw new InvalidOperationException($"unable to withdraw {amount}");
        }
        public Guid Deposit(decimal amount)
        {
            if (State.CanDeposit(amount))
            {
                var deposited = new AmountDeposited(Guid.NewGuid(), amount, DateTime.UtcNow);
                Emit(deposited);
                return deposited.TransactionId;
            }
            throw  new InvalidOperationException($"unable to deposit {amount}");
        }


    }
}