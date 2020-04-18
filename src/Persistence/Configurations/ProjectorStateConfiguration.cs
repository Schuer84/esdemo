using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Text;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Persistence.Configurations
{
    class ProjectorStateConfiguration : EntityTypeConfiguration<ProjectorState>
    {
        public ProjectorStateConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.Checkpoint);
            Property(x => x.LastUpdateUtc);
        }
    }
}
