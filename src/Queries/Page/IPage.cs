using System.Collections;
using System.Collections.Generic;

namespace SqlStreamStore.Demo.Queries.Page
{
    public interface IPage
    {
        int Count { get; }
        int Size { get; }
        int Index { get; }
        IEnumerable Items { get; }
    }

    public interface IPage<TModel> : IPage
    {
        new IEnumerable<TModel> Items { get; }
    }
}
