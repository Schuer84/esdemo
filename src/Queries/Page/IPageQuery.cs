namespace SqlStreamStore.Demo.Queries.Page
{
    public interface IPageQuery : IQuery
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }

    public interface IPageQuery<TEntity> : IQuery<TEntity>, IPageQuery
    {
        
    }
}