using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using SqlStreamStore.Demo.Queries.Page;

namespace SqlStreamStore.Demo.Queries
{

    public interface IQueryHandler
    {
       Task<IEnumerable> QueryList(IQuery query, CancellationToken cancellationToken);
       Task<IPage> QueryPage(IPageQuery query, CancellationToken cancellationToken);
    }

   
}