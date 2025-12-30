using Genetec.BookHistory.Entities.Responses;

namespace Genetec.BookHistory.Entities.RepositoryContracts
{
    public interface IBookRepository
    {
        Task<InsertBookResult?> Insert(string title, string shortDescription, DateOnly publishDate, IEnumerable<string> authors);

        Task<UpdateBookResult?> Update(int id, string? title, string? shortDescription, DateOnly? publishDate, IEnumerable<string>? authors, int lastRevisionNumber);

        Task Delete(int id);

        Task<GetBookResult?> Get(int id);
    }
}
