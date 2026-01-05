using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Requests;
using Genetec.BookHistory.Utilities;
using Genetec.BookHistory.Utilities.Extensions;
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
            var orders = request.Orders;
            if (orders.HasDuplicates())
            {
                return StatusCode(StatusCodes.Status400BadRequest, "There are duplicate columns in orders list");
            }

            var groups = request.Groups;
            if (groups.HasDuplicates())
            {
                return StatusCode(StatusCodes.Status400BadRequest, "There are duplicate columns in groups list");
            }

            if (orders?.Count() > 0 && groups?.Count() > 0)
            {
                var wrongOrders = orders.Where(item => !groups.Contains(item.Field));
                if (wrongOrders.Any())
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"The following order columns are not included in group columns: {ListJoiner.Get(wrongOrders.Select(item => item.Field))}");
                }
            }

            try
            {
                var result = await _bookHistoryRepository.Get(request.Filter, orders, request.PagingParameters, groups);
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
