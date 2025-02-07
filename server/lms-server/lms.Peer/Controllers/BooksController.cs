using AutoMapper;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using lms.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

using lms.Abstractions.Exceptions;
using lms.Abstractions.CustomActionFilters;

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository booksRepository;
        private IMapper mapper { get; }
        private readonly HttpClient httpClient;

        public BooksController(IBooksRepository booksRepository, IMapper mapper, HttpClient httpClient)
        {
            this.booksRepository = booksRepository;
            this.mapper = mapper;
            this.httpClient = httpClient;
        }

        [Authorize(Roles = "client,librarian")]
        [HttpGet("SearchBooks/{query}")]
        public async Task<ActionResult<BookDto>> SearchBooks([FromRoute] string query)
        {
            try
            {
                var books = await booksRepository.SearchBooksAsync(query);
                return Ok(mapper.Map<List<BookDto>>(books));
            }
            catch (Exception e)
            {
                throw new GlobalException($"in booksController: {e}");
                return StatusCode(500, "Internal server error");
            }
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
                throw new GlobalException($"in booksController: {e}");
                throw;

            }
        }

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
                throw new GlobalException($"in booksController: {e}");
            }
        }

        [Authorize(Roles = "librarian")]
        [HttpPost("AddBook")]
        public async Task<ActionResult<BookDto>> AddBook([FromBody] BookDto bookDtoObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var bookDomainModel = mapper.Map<Book>(bookDtoObject);

                bookDomainModel = await booksRepository.AddNewBook(bookDomainModel);

                var regionDto = mapper.Map<BookDto>(bookDomainModel);

                return CreatedAtAction(nameof(GetBookById), new { id = bookDomainModel.Id }, regionDto);
            }
            catch (GlobalException e)
            {
                return StatusCode(403, e.Message);
            }
            catch (Exception e)
            {
                throw new GlobalException($"in booksController: {e}");
            }
        }
        
        [Authorize(Roles = "librarian")]
        [HttpPut("UpdateBook/{id:Guid}")]
        public async Task<ActionResult<BookDto>> UpdateBook([FromRoute] Guid id,[FromBody] BookDto book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                throw new GlobalException($"in booksController: {e}");
                throw;
            }

        }
        [Authorize(Roles = "librarian")]
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
                throw new GlobalException($"in booksController: {e}");
                throw;

            }

        }
    }
}
