using System;

namespace SqlStreamStore.Demo.Events.Account
{
    public abstract class AccountEvent : Event
    {
        public Guid AccountId { get; set; }

        protected AccountEvent(string type) 
            : base(type)
        { }
    }
}