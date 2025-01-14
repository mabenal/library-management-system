using AutoMapper;
using lms.Abstractions.Models.DTO;
using lms_server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace lms_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository booksRepository;
        private IMapper mapper { get; }

        public BooksController(IBooksRepository booksRepository, IMapper mapper)
        {
            this.booksRepository = booksRepository;
            this.mapper = mapper;
        }

        //public async Task<IActionResult> GetBook()
        //{
        //    return Ok();
        //}

        [HttpGet("GetAllBooks")]
        public async Task<ActionResult<BookDto>> GetAllBooks()
        {
            try
            {
                var books = await booksRepository.GetAllBooksAsync();
                return Ok(mapper.Map<List<BookDto>>(books));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"in booksController: {e}");
                throw;
            }
        }

        //public async Task<IActionResult> AddBook()
        //{
        //    return Created();
        //}

        //public async Task<IActionResult> RequestBook()
        //{
        //    return Ok();
        //}

        //public async Task<IActionResult> AssignBook()

        //    return Ok();
        //}

    }
}
