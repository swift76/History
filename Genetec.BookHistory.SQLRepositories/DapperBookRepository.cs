using Dapper;
using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Utilities.Extensions;
using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Responses;
using Genetec.BookHistory.SQLRepositories.Base;

namespace Genetec.BookHistory.SQLRepositories
{
    public class DapperBookRepository(string connectionString) : DapperBaseRepository(connectionString), IBookRepository
    {
        public async Task<InsertBookResult?> Insert(string title, string shortDescription, DateOnly publishDate, IEnumerable<string> authors)
        {
            DynamicParameters parameters = new();
            parameters.Add("Title", title);
            parameters.Add("ShortDescription", shortDescription);
            parameters.Add("PublishDate", publishDate.ConvertToDateTime());
            AddSystemTableValuedParameter(parameters, "Name", "Authors", authors);
            return await GetSingleAsync<InsertBookResult>(parameters, "gn0sp_InsertBook");
        }

        public async Task<UpdateBookResult?> Update(int id, string? title, string? shortDescription, DateOnly? publishDate, IEnumerable<string>? authors, int lastRevisionNumber)
        {
            DynamicParameters parameters = new();
            parameters.Add("Id", id);
            if (title != null)
            {
                parameters.Add("Title", title);
            }
            if (shortDescription != null)
            {
                parameters.Add("ShortDescription", shortDescription);
            }
            if (publishDate.HasValue)
            {
                parameters.Add("PublishDate", publishDate.Value.ConvertToDateTime());
            }
            if (authors != null && authors.Any())
            {
                AddSystemTableValuedParameter(parameters, "Name", "Authors", authors);
            }
            parameters.Add("RevisionNumber", lastRevisionNumber);
            return await GetSingleAsync<UpdateBookResult>(parameters, "gn0sp_UpdateBook");
        }

        public async Task Delete(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("Id", id);
            await ExecuteAsync(parameters, "gn0sp_DeleteBook");
        }

        public async Task<GetBookResult?> Get(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("Id", id);
            var result = await GetSingleAsync<GetBookResult>(parameters, "gn0sp_GetBook");
            if (result != null)
            {
                parameters = new();
                parameters.Add("BookId", id);
                result.Authors = await GetListAsync<Author>(parameters, "gn0sp_GetAuthors");
            }

            return result;
        }
    }
}
