using System;
using System.Threading.Tasks;

namespace LiquidProjections.EntityFramework
{
    internal sealed class EntityFrameworkEventMapConfigurator<TProjection, TKey>
        where TProjection : class, new()
    {
        private readonly Action<TProjection, TKey> _setIdentity;
        private readonly IEventMap<EntityFrameworkProjectionContext> _eventMap;
        private readonly IEventMapBuilder<TProjection, TKey, EntityFrameworkProjectionContext> _eventMapBuilder;

        public EntityFrameworkEventMapConfigurator(
            IEventMapBuilder<TProjection, TKey, EntityFrameworkProjectionContext> eventMapBuilder,
            Action<TProjection, TKey> setIdentity)
        {
            _setIdentity = setIdentity ?? throw new ArgumentNullException(nameof(setIdentity));
            _eventMapBuilder = eventMapBuilder ?? throw new ArgumentNullException(nameof(eventMapBuilder));
            _eventMap = BuildEventMap(eventMapBuilder);
        }

        public Predicate<TProjection> Filter { get; set; } = _ => true;

        public async Task ProjectEvent(object anEvent, EntityFrameworkProjectionContext context)
        {
            context.WasHandled = await _eventMap.Handle(anEvent, context).ConfigureAwait(false);
        }

        private IEventMap<EntityFrameworkProjectionContext> BuildEventMap(
            IEventMapBuilder<TProjection, TKey, EntityFrameworkProjectionContext> mapBuilder)
        {
            return mapBuilder.Build(new ProjectorMap<TProjection, TKey, EntityFrameworkProjectionContext>
            {
                Create = OnCreate,
                Update = OnUpdate,
                Delete = OnDelete,
                Custom = (context, projector) => projector()
            });
        }

        private async Task OnCreate(TKey key, EntityFrameworkProjectionContext context, Func<TProjection, Task> projector, Func<TProjection, bool> shouldOverwrite)
        {
            TProjection projection = await context.GetProjection<TProjection>(key); 
            if ((projection == null) || shouldOverwrite(projection))
            {
                if (projection == null)
                {
                    projection = new TProjection();
                    _setIdentity(projection, key);
                    await projector(projection).ConfigureAwait(false);

                    var set = context.GetDbSet<TProjection>();
                        set.Add(projection);
                    
                    //await context.SaveAsync().ConfigureAwait(false);
                }
                else
                {
                    await projector(projection).ConfigureAwait(false);
                }
            }
        }

        private async Task OnUpdate(TKey key, EntityFrameworkProjectionContext context, Func<TProjection, Task> projector, Func<bool> createIfMissing)
        {
            TProjection projection = await context.GetProjection<TProjection>(key);
            if ((projection == null) && createIfMissing())
            {
                projection = new TProjection();
                _setIdentity(projection, key);
                await projector(projection).ConfigureAwait(false);
                //await context.SaveAsync().ConfigureAwait(false);
            }
            else
            {
                if (projection != null && Filter(projection))
                {
                    await projector(projection).ConfigureAwait(false);
                }
            }
        }

        private async Task<bool> OnDelete(TKey key, EntityFrameworkProjectionContext context)
        {
            TProjection existingProjection = await context.GetProjection<TProjection>(key);
            if (existingProjection != null)
            {
                var set = context.GetDbSet<TProjection>();
                    set.Remove(existingProjection);
                //await context.SaveAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }
    }
}
