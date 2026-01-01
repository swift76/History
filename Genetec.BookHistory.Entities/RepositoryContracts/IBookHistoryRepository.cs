using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Filters;
using Genetec.BookHistory.Entities.Orders;
using Genetec.BookHistory.Entities.Paging;
using Genetec.BookHistory.Entities.Responses;

namespace Genetec.BookHistory.Entities.RepositoryContracts
{
    public interface IBookHistoryRepository
    {
        Task<IEnumerable<BookHistoryResult>> Get(BookHistoryFilter? filter = null
            , IEnumerable<BookHistoryOrder>? orders = null
            , PagingParameters? pagingParameters = null
            , IEnumerable<BookHistoryField>? groups = null);
    }
}
