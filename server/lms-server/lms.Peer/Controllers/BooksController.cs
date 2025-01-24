using AutoMapper;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using lms.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository booksRepository;
        private IMapper mapper { get; }
        private readonly string apiKey = "AIzaSyCKPZUJfOAlBng9idsWRRptw2e-MDX5x2M";
        private readonly HttpClient httpClient;

        public BooksController(IBooksRepository booksRepository, IMapper mapper, HttpClient httpClient)
        {
            this.booksRepository = booksRepository;
            this.mapper = mapper;
            this.httpClient = httpClient;
        }

        [HttpGet("SearchBooks/{query}")]
        public async Task<ActionResult> SearchBooks([FromRoute] string query)
        {
            try
            {
                var response = await httpClient.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=subject:{query}&maxResults=10&key={apiKey}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(content);

                var filteredBooks = jsonDocument.RootElement.GetProperty("items").EnumerateArray().Select(item =>
                {
                    var volumeInfo = item.GetProperty("volumeInfo");

                    volumeInfo.TryGetProperty("title", out var titleElement);
                    volumeInfo.TryGetProperty("authors", out var authorsElement);
                    volumeInfo.TryGetProperty("publisher", out var publisherElement);
                    volumeInfo.TryGetProperty("publishedDate", out var publishedDateElement);
                    volumeInfo.TryGetProperty("description", out var descriptionElement);
                    volumeInfo.TryGetProperty("industryIdentifiers", out var industryIdentifiersElement);
                    volumeInfo.TryGetProperty("pageCount", out var pageCountElement);
                    volumeInfo.TryGetProperty("categories", out var categoriesElement);
                    volumeInfo.TryGetProperty("imageLinks", out var imageLinksElement);

                    return new {
                        title = titleElement.ValueKind == JsonValueKind.String ? titleElement.GetString() : null,
                        authors = authorsElement.ValueKind == JsonValueKind.Array 
                            ? authorsElement.GetArrayLength() == 1 
                                ? authorsElement.EnumerateArray().First().GetString() 
                                : string.Join(", ", authorsElement.EnumerateArray().Select(author => author.GetString()).ToArray(), 0, authorsElement.GetArrayLength() - 1) + " & " + authorsElement.EnumerateArray().Last().GetString() 
                            : null,
                        publisher = publisherElement.ValueKind == JsonValueKind.String ? publisherElement.GetString() : null,
                        publishedDate = publishedDateElement.ValueKind == JsonValueKind.String ? publishedDateElement.GetString() : null,
                        description = descriptionElement.ValueKind == JsonValueKind.String ? descriptionElement.GetString() : null,
                        isbn = industryIdentifiersElement.ValueKind == JsonValueKind.Array 
                            ? industryIdentifiersElement.EnumerateArray()
                                .Select(identifier => identifier.GetProperty("identifier").GetString())
                                .OrderByDescending(id => id.Length)
                                .FirstOrDefault() 
                            : null,
                        pageCount = pageCountElement.ValueKind == JsonValueKind.Number ? pageCountElement.GetInt32() : (int?)null,
                        categories = categoriesElement.ValueKind == JsonValueKind.Array 
                            ? categoriesElement.GetArrayLength() == 1 
                                ? categoriesElement.EnumerateArray().First().GetString() 
                                : string.Join(", ", categoriesElement.EnumerateArray().Select(category => category.GetString()).ToArray(), 0, categoriesElement.GetArrayLength() - 1) + " & " + categoriesElement.EnumerateArray().Last().GetString() 
                            : null,
                        thumbnail = imageLinksElement.ValueKind == JsonValueKind.Object && imageLinksElement.TryGetProperty("thumbnail", out var thumbnailElement) 
                            ? thumbnailElement.GetString() 
                            : null,
                        numberOfCopies = new Random().Next(1, 26)
                    };
                }).ToList();

                //// Read existing JSON file
                //var jsonFilePath = "../../lms-server/lms-server/books.json";
                //var existingJson = await System.IO.File.ReadAllTextAsync(jsonFilePath);
                //var existingBooks = JsonSerializer.Deserialize<List<object>>(existingJson);

                //// Append new search results
                //existingBooks.AddRange(filteredBooks);

                //// Write updated data back to JSON file
                //var updatedJson = JsonSerializer.Serialize(existingBooks, new JsonSerializerOptions { WriteIndented = true });
                //await System.IO.File.WriteAllTextAsync(jsonFilePath, updatedJson);

                return Ok(filteredBooks);
            }
            catch (HttpRequestException e)
            {
                Console.Error.WriteLine($"Request error: {e.Message}");
                return StatusCode(500, "Internal server error");
            }
            catch (JsonException e)
            {
                Console.Error.WriteLine($"JSON error: {e.Message}");
                return StatusCode(500, "Error processing JSON response");
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
                Console.Error.WriteLine($"in booksController: {e}");
                throw;

            }
        }
        [Authorize(Roles = "client")]
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

        [Authorize(Roles = "librarian")]
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
        [Authorize(Roles = "librarian")]
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
                Console.Error.WriteLine($"in booksController: {e}");
                throw;

            }

        }
    }
}
