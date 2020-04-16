using System.Collections;
using System.Collections.Generic;

namespace SqlStreamStore.Demo.Queries.Page
{
    public class Page<TModel> : IPage<TModel>
    {
        public int Count { get; set; }
        public int Size { get; set; }
        public int Index { get; set; }
        public IEnumerable<TModel> Items { get; set; }

        IEnumerable IPage.Items => Items;
    }
}