using System;
using System.Collections.Generic;
using SqlStreamStore.Demo.Events.Account;

namespace SqlStreamStore.Demo.Aggregates.Account
{
    public class AccountAggregateState : AggregateStateBase
    {
        private Balance Balance { get; } = new Balance(0, DateTime.UtcNow);
        private IEnumerable<Person> Owners { get; } = new List<Person>();


        public bool CanWithdraw(decimal amount)
        {
            return Balance.Amount - amount >= 0;
        }

        public bool CanDeposit(decimal amount)
        {
            return amount > 0;
        }


        void On(AmountWithdrawn withdrawn)
        {
            Balance.Subtract(withdrawn.Amount);
        }

        void On(AmountDeposited deposit)
        {
            Balance.Add(deposit.Amount);
        }
    }
}