using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Requests;
using Genetec.BookHistory.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Genetec.BookHistory.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController(IBookRepository bookRepository) : ControllerBase
    {
        private readonly IBookRepository _bookRepository = bookRepository;

        [HttpPost(Name = "Insert")]
        public async Task<IActionResult> Insert([FromBody] InsertBook book)
        {
            var authors = book.Authors.GetNormalizedAuthors();
            if (authors?.Count() == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Authors are not specified");
            }

            try
            {
                var result = await _bookRepository.Insert(book.Title?.Trim(), book.ShortDescription?.Trim(), book.PublishDate.Value, authors);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch(Name = "Update")]
        public async Task<IActionResult> Update([FromBody] UpdateBook book)
        {
            try
            {
                var currentBook = await _bookRepository.Get(book.Id);
                if (currentBook == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "The book with the specified ID doesn't exist");
                }

                if (currentBook.IsDeleted)
                {
                    return StatusCode(StatusCodes.Status410Gone, "The book with the specified ID is deleted");
                }

                if (currentBook.RevisionNumber != book.LastRevisionNumber)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "The last revision number is wrong");
                }

                var updatedTitle = book.Title.GetUpdatedValue(currentBook.Title);
                var updatedShortDescription = book.ShortDescription.GetUpdatedValue(currentBook.ShortDescription);
                var updatedPublishDate = book.PublishDate.GetUpdatedValue(currentBook.PublishDate);
                var updatedAuthors = book.Authors.GetUpdatedValue(currentBook.Authors);

                if (updatedTitle == null 
                    && updatedShortDescription == null
                    && updatedPublishDate == null
                    && updatedAuthors == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "No data change is detected");
                }

                var result = await _bookRepository.Update(book.Id
                    , updatedTitle
                    , updatedShortDescription
                    , updatedPublishDate
                    , updatedAuthors
                    , book.LastRevisionNumber);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete(Name = "Delete")]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            try
            {
                var currentBook = await _bookRepository.Get(id);
                if (currentBook == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "The book with the specified ID doesn't exist");
                }

                if (currentBook.IsDeleted)
                {
                    return StatusCode(StatusCodes.Status410Gone, "The book with the specified ID is deleted");
                }

                await _bookRepository.Delete(id);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
