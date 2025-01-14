using AutoMapper;
using lms_server.Models.DTO;
using lms_server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lms_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository booksRepository;

        public IMapper mapper { get; }

        public BooksController(IBooksRepository booksRepository, IMapper mapper)
        {
            this.booksRepository = booksRepository;
            this.mapper = mapper;
        }

        //public async Task<IActionResult> GetBook()
        //{
        //    return Ok();
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await booksRepository.GetAllBooksAsync();


            return Ok(mapper.Map<List<BookDto>>(books));
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
