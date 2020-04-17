using System;

namespace SqlStreamStore.Demo.Commands.Account
{
    public abstract class AccountCommand : ICommand
    {
        public Guid AccountId { get; set; } = Guid.Parse("5af8872f-fd5c-4599-ac0d-29ddf400d823");
        public virtual string GetAggregateId()
        {
            return $"Account:{AccountId:D}";
        }
    }
}