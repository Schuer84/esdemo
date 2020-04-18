using System.Data.Entity.ModelConfiguration;
using SqlStreamStore.Demo.Persistence.Entities;

namespace SqlStreamStore.Demo.Persistence.Configurations
{
    public abstract class EntityConfiguration<TEntity> : EntityTypeConfiguration<TEntity> 
        where TEntity: Entity
    {
        protected EntityConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.CreatedOn);
        }
    }
}