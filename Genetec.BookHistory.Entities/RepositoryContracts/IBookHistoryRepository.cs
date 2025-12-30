using Genetec.BookHistory.Entities.Responses;

namespace Genetec.BookHistory.Entities.RepositoryContracts
{
    public interface IBookHistoryRepository
    {
        Task<IEnumerable<BookHistoryResult>> Get(int? pageNumber = null, int? pageSize = null);
    }
}
