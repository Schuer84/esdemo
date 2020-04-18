using System;
using System.Collections.Generic;
using System.Text;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Persistence.Configurations
{
    public class UserConfiguration : EntityConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(x => x.Email);
            Property(x => x.FamilyName);
            Property(x => x.GivenName);
        }
    }
}
