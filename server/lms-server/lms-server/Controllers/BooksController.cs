using AutoMapper;
using lms_server.Models;
using lms_server.Models.DTO;
using lms_server.Repository;
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

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBookById([FromRoute] Guid id)
        {
            var book = await booksRepository.GetBookById(id);

            return Ok(mapper.Map<BookDto>(book));

        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await booksRepository.GetAllBooksAsync();


            return Ok(mapper.Map<List<BookDto>>(books));
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookDto bookDtoObject)
        {
            var bookDomainModel = mapper.Map<Book>(bookDtoObject);

            bookDomainModel= await booksRepository.AddNewBook(bookDomainModel);

            var regionDto = mapper.Map<BookDto>(bookDomainModel);

            return CreatedAtAction(nameof(GetBookById), new { id = bookDomainModel.Id }, regionDto);

        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateBook([FromRoute] Guid id,[FromBody] BookDto book)
        {
            var bookDomainModel = mapper.Map<Book>(book);

            bookDomainModel = await booksRepository.UpdateNewBook(id, bookDomainModel);

            if(bookDomainModel == null)
            {
                return NotFound();
            }

            var bookDto = mapper.Map<BookDto>(bookDomainModel);

            return Ok(bookDto);
        }

    

    }
}
