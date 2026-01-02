using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Responses;
using Genetec.BookHistory.PostgreRepositories.Base;
using Genetec.BookHistory.PostgreRepositories.Data;
using Genetec.BookHistory.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Genetec.BookHistory.PostgreRepositories
{
    public class EFBookRepository(string connectionString) : EFBaseRepository(connectionString), IBookRepository
    {
        public async Task<InsertBookResult?> Insert(string title, string shortDescription, DateOnly publishDate, IEnumerable<string> authors)
        {
            await using var context = CreateContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var book = new BookData
                {
                    Title = title,
                    ShortDescription = shortDescription,
                    PublishDate = publishDate,
                    Authors = [.. authors],
                    RevisionNumber = 1
                };
                context.Books.Add(book);
                await context.SaveChangesAsync();

                context.BookHistories.Add(new BookHistoryData
                {
                    BookId = book.Id,
                    OperationDate = DateTime.UtcNow,
                    OperationId = (byte)BookOperation.Insert,
                    Title = book.Title,
                    ShortDescription = book.ShortDescription,
                    PublishDate = book.PublishDate,
                    Authors = book.Authors
                });

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new InsertBookResult { Id = book.Id, RevisionNumber = book.RevisionNumber };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                throw new ApplicationException("Error inserting book: " + ex.Message, ex);
            }
        }

        public async Task<UpdateBookResult?> Update(int id, string? title, string? shortDescription, DateOnly? publishDate, IEnumerable<string>? authors, int lastRevisionNumber)
        {
            await using var context = CreateContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var book = await context.Books.FirstOrDefaultAsync(item => item.Id == id) ?? throw new InvalidOperationException("Book not found");
                if (book.IsDeleted)
                {
                    throw new InvalidOperationException("Book is deleted");
                }

                if (book.RevisionNumber != lastRevisionNumber)
                {
                    throw new DbUpdateConcurrencyException();
                }

                if (title != null)
                {
                    book.Title = title;
                }

                if (shortDescription != null)
                {
                    book.ShortDescription = shortDescription;
                }

                if (publishDate.HasValue)
                {
                    book.PublishDate = publishDate.Value;
                }

                if (authors != null)
                {
                    book.Authors = [.. authors];
                }

                book.RevisionNumber++;

                context.BookHistories.Add(new BookHistoryData
                {
                    BookId = book.Id,
                    OperationDate = DateTime.UtcNow,
                    OperationId = (byte)BookOperation.Update,
                    Title = book.Title,
                    ShortDescription = book.ShortDescription,
                    PublishDate = book.PublishDate,
                    Authors = book.Authors
                });

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new UpdateBookResult { RevisionNumber = book.RevisionNumber };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                throw new ApplicationException("Error updating book: " + ex.Message, ex);
            }
        }

        public async Task Delete(int id)
        {
            await using var context = CreateContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var book = await context.Books.FirstOrDefaultAsync(item => item.Id == id) ?? throw new InvalidOperationException("Book not found");
                if (book.IsDeleted)
                {
                    throw new InvalidOperationException("Book is deleted");
                }

                book.IsDeleted = true;

                context.BookHistories.Add(new BookHistoryData
                {
                    BookId = book.Id,
                    OperationDate = DateTime.UtcNow,
                    OperationId = (byte)BookOperation.Delete
                });

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                throw new ApplicationException("Error deleting book: " + ex.Message, ex);
            }
        }

        public async Task<GetBookResult?> Get(int id)
        {
            await using var context = CreateContext();

            try
            {
                var book = await context.Books
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item => item.Id == id);

                if (book == null)
                {
                    return null;
                }

                return new GetBookResult
                {
                    Title = book.Title,
                    ShortDescription = book.ShortDescription,
                    PublishDate = book.PublishDate.ConvertToDateTime(),
                    Authors = book.Authors.Select(item => new Author()
                    {
                        Name = item
                    }),
                    IsDeleted = book.IsDeleted,
                    RevisionNumber = book.RevisionNumber
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting book: " + ex.Message, ex);
            }
        }
    }
}
