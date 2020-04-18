using System;

namespace SqlStreamStore.Demo.Persistence.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}