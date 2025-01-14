using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lms_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        public BooksController()
        {
            
        }

        public async Task<IActionResult> GetBook()
        {
            return Ok(); 
       }

        public async Task<IActionResult> GetAllBooks()
        {
            return Ok();
        }

        public async Task<IActionResult> AddBook()
        {
            return Created();
        }

        public async Task<IActionResult> RequestBook()
        {
            return Ok();
        }

        public async Task<IActionResult> AssignBook()
        {
            return Ok();
        }

    }
}
