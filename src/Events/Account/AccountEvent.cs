using System;
using SqlStreamStore.Demo.Serializers.Messages;

namespace SqlStreamStore.Demo.Events.Account
{
    public abstract class AccountEvent : Event
    {
        public Guid AccountId { get; set; }

        protected AccountEvent(string type) 
            : base(type)
        { }
    }

    public class AccountRegisteredEvent : AccountEvent
    {
        public AccountRegisteredEvent()
            : base(MessageTypes.Account.Registered)
        { }
        public Guid UserId { get; set; }
    }
}