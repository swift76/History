using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Requests;
using Genetec.BookHistory.Utilities;
using Microsoft.AspNetCore.Mvc;

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
            if (request.Orders?.Count() > 0 && request.Groups?.Count() > 0)
            {
                var wrongOrders = request.Orders.Where(item => !request.Groups.Contains(item.Field));
                if (wrongOrders.Any())
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"The following order columns are not included in group columns: {ListJoiner.Get(wrongOrders.Select(item => item.Field))}");
                }
            }

            try
            {
                var result = await _bookHistoryRepository.Get(request.Filter, request.Orders, request.PagingParameters, request.Groups);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
