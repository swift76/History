using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Filters;
using Genetec.BookHistory.Entities.Orders;
using Genetec.BookHistory.Entities.Paging;
using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Responses;
using Genetec.BookHistory.PostgreRepositories.Base;

namespace Genetec.BookHistory.PostgreRepositories
{
    public class EFBookHistoryRepository(string connectionString) : EFBaseRepository(connectionString), IBookHistoryRepository
    {
        public async Task<IEnumerable<BookHistoryResult>> Get(BookHistoryFilter? filter = null
           , IEnumerable<BookHistoryOrder>? orders = null
           , PagingParameters? pagingParameters = null
           , IEnumerable<BookHistoryField>? groups = null)
        {
            throw new NotImplementedException();
        }
    }
}
