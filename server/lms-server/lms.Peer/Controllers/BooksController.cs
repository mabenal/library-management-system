using AutoMapper;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using lms.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace lms.Peer.Controllers
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

        [HttpGet("GetBook/{id:Guid}")]
        public async Task<ActionResult<BookDto>> GetBookById([FromRoute] Guid id)
        {
            try
            {
                var book = await booksRepository.GetBookById(id);

                return Ok(mapper.Map<BookDto>(book));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"in booksController: {e}");
                throw;

            }
        }
        [Authorize(Roles = "Client")]
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

        [HttpPost("AddBook")]
        public async Task<ActionResult<BookDto>> AddBook([FromBody] BookDto bookDtoObject)
        {
            try
            {
                var bookDomainModel = mapper.Map<Book>(bookDtoObject);

                bookDomainModel = await booksRepository.AddNewBook(bookDomainModel);

                var regionDto = mapper.Map<BookDto>(bookDomainModel);

                return CreatedAtAction(nameof(GetBookById), new { id = bookDomainModel.Id }, regionDto);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"in booksController: {e}");
                throw;
            }


        }

        [HttpPut("UpdateBook/{id:Guid}")]
        public async Task<ActionResult<BookDto>> UpdateBook([FromRoute] Guid id,[FromBody] BookDto book)
        {
            try
            {
                var bookDomainModel = mapper.Map<Book>(book);

                bookDomainModel = await booksRepository.UpdateNewBook(id, bookDomainModel);

                if (bookDomainModel == null)
                {
                    return NotFound();
                }

                var bookDto = mapper.Map<BookDto>(bookDomainModel);

                return Ok(bookDto);

            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"in booksController: {e}");
                throw;
            }

        }
        [HttpDelete("RemoveBook/{id:Guid}")]
        public async Task<ActionResult<BookDto>> DeleteBook([FromRoute] Guid id)
        {
            try
            {
                var book = await booksRepository.DeleteBookAsync(id);

                if(book == null)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<BookDto>(book));


            }catch(Exception e)
            {
                Console.Error.WriteLine($"in booksController: {e}");
                throw;

            }

        }
    }
}
