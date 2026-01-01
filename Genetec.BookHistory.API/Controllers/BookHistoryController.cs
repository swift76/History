using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Extensions;
using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Requests;
using Genetec.BookHistory.Entities.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Genetec.BookHistory.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookHistoryController(IBookHistoryRepository bookHistoryRepository) : ControllerBase
    {
        private readonly IBookHistoryRepository _bookHistoryRepository = bookHistoryRepository;

        [HttpPost(Name = "Get")]
        public async Task<IActionResult> Get([FromBody] GetBookHistory request)
        {
            try
            {
                var result = await _bookHistoryRepository.Get(request.Filter, request.Orders, request.PagingParameters);
                return StatusCode(StatusCodes.Status200OK, result.Select(item => new BookHistoryResult()
                {
                    Id = item.Id,
                    BookId = item.BookId,
                    OperationDate = item.OperationDate,
                    OperationId = (BookOperation)item.OperationId,
                    Title = item.Title,
                    ShortDescription = item.ShortDescription,
                    PublishDate = item.PublishDate.ConvertToDateOnly(),
                    Authors = (item.Authors == null ? null : JsonConvert.DeserializeObject<IEnumerable<Author>>(item.Authors))
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
