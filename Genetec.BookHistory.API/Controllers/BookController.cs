using Genetec.BookHistory.Entities.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Genetec.BookHistory.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        [HttpPost(Name = "Insert")]
        public async Task<IActionResult> Insert([FromBody] InsertBook book)
        {
            var authors = book.Authors?.Where(item => !string.IsNullOrEmpty(item));
            if (authors?.Count() == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Authors are not specified");
            }

            try
            {
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
