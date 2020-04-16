using System;

namespace SqlStreamStore.Demo.Commands.Account
{
    public abstract class AccountCommand : ICommand
    {
        public Guid AccountId { get; set; }
    }
}