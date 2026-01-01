using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Entities.Filters;
using Genetec.BookHistory.Entities.Orders;
using Genetec.BookHistory.Entities.Paging;

namespace Genetec.BookHistory.Entities.RepositoryContracts
{
    public interface IBookHistoryRepository
    {
        Task<IEnumerable<BookHistoryDto>> Get(BookHistoryFilter? filter = null, IEnumerable<BookHistoryOrder>? orders = null, PagingParameters? pagingParameters = null);
    }
}
