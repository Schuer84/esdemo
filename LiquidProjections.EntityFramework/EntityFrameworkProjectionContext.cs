using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace LiquidProjections.EntityFramework
{
    public class EntityFrameworkProjectionContext : ProjectionContext
    {
        public EntityFrameworkProjectionContext(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public DbContext DbContext { get; }
        public bool WasHandled { get; set; }

        internal async Task<TProjection> GetProjection<TProjection>(object key) where TProjection: class
        {
            //todo: what about includes etc? FindAsync won't suffice I assume
            return await GetDbSet<TProjection>().FindAsync(key);
        }

        internal DbSet<TProjection> GetDbSet<TProjection>() where TProjection : class
        {
            return DbContext.Set<TProjection>();
        }

        internal async Task<int> SaveAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }
}
