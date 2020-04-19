using System;
using SqlStreamStore.Demo.Serializers.Messages;

namespace SqlStreamStore.Demo.Events.Account
{
    public class AccountRegisteredEvent : AccountEvent
    {
        public AccountRegisteredEvent()
            : base(MessageTypes.Account.Registered)
        { }
        public Guid UserId { get; set; }
    }
}