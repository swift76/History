using BookHistory.Entities.Enums;
using BookHistory.Entities.Filters;
using BookHistory.Entities.Orders;
using BookHistory.Entities.Paging;
using BookHistory.Entities.Responses;

namespace BookHistory.Entities.RepositoryContracts
{
    public interface IBookHistoryRepository
    {
        Task<IEnumerable<BookHistoryResult>> Get(BookHistoryFilter? filter = null
            , IEnumerable<BookHistoryOrder>? orders = null
            , PagingParameters? pagingParameters = null
            , IEnumerable<BookHistoryField>? groups = null);
    }
}
