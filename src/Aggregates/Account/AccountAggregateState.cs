using System;

namespace SqlStreamStore.Demo
{
    public class AccountAggregateState : AggregateStateBase
    {
        protected Balance Balance { get; } = new Balance(0, DateTime.UtcNow);


        public bool CanWithdraw(decimal amount)
        {
            return Balance.Amount - amount > 0;
        }

        public bool CanDeposit(decimal amount)
        {
            return amount > 0;
        }


        void On(Withdrawn withdrawn)
        {
            Balance.Subtract(withdrawn.Amount);
        }

        void On(Deposited deposit)
        {
            Balance.Add(deposit.Amount);
        }
    }
}