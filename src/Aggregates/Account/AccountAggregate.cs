using System;
using System.Threading;
using System.Threading.Tasks;

namespace SqlStreamStore.Demo
{
    public class AccountAggregate : AggregateBase<AccountAggregateState>
    {
        public AccountAggregate(IEventStore eventStore) 
            : base(eventStore)
        { }

        public async Task<Guid> WithdrawAsync(decimal amount, CancellationToken cancellationToken)
        {
            if (State.CanWithdraw(amount))
            {
                var withdrawEvent = new Withdrawn(Guid.NewGuid(), amount, DateTime.UtcNow);
                await EmitAsync(withdrawEvent, cancellationToken);
                return withdrawEvent.TransactionId;
            }
            throw new InvalidOperationException($"unable to withdraw {amount}");
        }
        public async Task<Guid> DepositAsync(decimal amount, CancellationToken cancellationToken)
        {
            if (State.CanDeposit(amount))
            {
                var deposited = new Deposited(Guid.NewGuid(), amount, DateTime.UtcNow);
                await EmitAsync(deposited, cancellationToken);
                return deposited.TransactionId;
            }
            throw  new InvalidOperationException($"unable to deposit {amount}");
        }


    }
}