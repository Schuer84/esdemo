using System;
using System.Linq;

namespace SqlStreamStore.Demo.Queries.Account
{
    public abstract class Query<T> : IQuery<T>
    {
        public abstract IQueryable<T> GetQuery(IQueryable<T> source);

        IQueryable IQuery.GetQuery(IQueryable source)
        {
            var converted = source as IQueryable<T>;
            if (converted == null)
            {
                throw new InvalidCastException();
            }

            return GetQuery(converted);
        }
    }
}