using System;

namespace SqlStreamStore.Demo.Commands.Account
{
    public abstract class AccountCommand : ICommand
    {
        public Guid AccountId { get; set; } = Guid.NewGuid();
        
        public virtual string GetAggregateId()
        {
            return $"Account:{AccountId:D}";
        }
    }
}