using Xunit;
using AutoMapper;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Abstractions.Mappings;
using System;

namespace lms.Tests.AbstractionsTests.Mappings
{
    public class AutoMappingTests
    {
        private readonly IMapper _mapper;

        public AutoMappingTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfiles>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void ShouldMapBookToBookDto()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                YearPublished = "1998"
            };

            // Act
            var bookDto = _mapper.Map<BookDto>(book);

            // Assert
            Assert.NotNull(bookDto);
            Assert.Equal(book.Id, bookDto.Id);
            Assert.Equal(book.Title, bookDto.Title);
            Assert.Equal(book.Author, bookDto.Author);
            Assert.Equal(book.ISBN, bookDto.ISBN);
            Assert.Equal(book.YearPublished, bookDto.YearPublished);
        }

        [Fact]
        public void ShouldMapBookDtoToBook()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Id = Guid.NewGuid(),
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                YearPublished = "1998"
            };

            // Act
            var book = _mapper.Map<Book>(bookDto);

            // Assert
            Assert.NotNull(book);
            Assert.Equal(bookDto.Id, book.Id);
            Assert.Equal(bookDto.Title, book.Title);
            Assert.Equal(bookDto.Author, book.Author);
            Assert.Equal(bookDto.ISBN, book.ISBN);
            Assert.Equal(bookDto.YearPublished, book.YearPublished);
        }

        [Fact]
        public void ShouldMapClientToClientDto()
        {
            // Arrange
            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = "Test Client",
                LastName = "Test LastName",
                EmailAddress = "test@example.com",
                Password = "password",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            // Act
            var clientDto = _mapper.Map<ClientDto>(client);

            // Assert
            Assert.NotNull(clientDto);
            Assert.Equal(client.Id, clientDto.Id);
            Assert.Equal(client.Name, clientDto.Name);
            Assert.Equal(client.LastName, clientDto.LastName);
            Assert.Equal(client.EmailAddress, clientDto.EmailAddress);
            Assert.Equal(client.Password, clientDto.Password);
            Assert.Equal(client.Address, clientDto.Address);
            Assert.Equal(client.PhoneNumber, clientDto.PhoneNumber);
        }

        [Fact]
        public void ShouldMapClientDtoToClient()
        {
            // Arrange
            var clientDto = new ClientDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Client",
                LastName = "Test LastName",
                EmailAddress = "test@example.com",
                Password = "password",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            // Act
            var client = _mapper.Map<Client>(clientDto);

            // Assert
            Assert.NotNull(client);
            Assert.Equal(clientDto.Id, client.Id);
            Assert.Equal(clientDto.Name, client.Name);
            Assert.Equal(clientDto.LastName, client.LastName);
            Assert.Equal(clientDto.EmailAddress, client.EmailAddress);
            Assert.Equal(clientDto.Password, client.Password);
            Assert.Equal(clientDto.Address, client.Address);
            Assert.Equal(clientDto.PhoneNumber, client.PhoneNumber);
        }

        [Fact]
        public void ShouldMapBookRequestToBookRequestDto()
        {
            // Arrange
            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = "Test Client",
                LastName = "Test LastName",
                EmailAddress = "test@example.com",
                Password = "password",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                YearPublished = "1998"
            };

            var bookRequest = BookRequest.CreatePendingRequest(
                "Test Book Request",
                DateTime.Now,
                DateTime.Now.AddDays(7),
                client,
                book
            );

            // Act
            var bookRequestDto = _mapper.Map<BookRequestDto>(bookRequest);

            // Assert
            Assert.NotNull(bookRequestDto);
            Assert.Equal(bookRequest.Title, bookRequestDto.Title);
            Assert.Equal(bookRequest.DateRequested, bookRequestDto.DateRequested);
            Assert.Equal(bookRequest.AcceptedReturnDate, bookRequestDto.AcceptedReturnDate);
            Assert.Equal(bookRequest.ClientId, bookRequestDto.ClientId);
            Assert.Equal(bookRequest.BookId, bookRequestDto.BookId);
        }

        [Fact]
        public void ShouldMapBookRequestDtoToBookRequest()
        {
            // Arrange
            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = "Test Client",
                LastName = "Test LastName",
                EmailAddress = "test@example.com",
                Password = "password",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                YearPublished = "1998"
            };

            var bookRequestDto = new BookRequestDto
            {
                Title = "Test Book Request",
                DateRequested = DateTime.Now,
                AcceptedReturnDate = DateTime.Now.AddDays(7),
                ClientId = client.Id,
                BookId = book.Id
            };

            // Act
            var bookRequest = _mapper.Map<BookRequest>(bookRequestDto);

            // Assert
            Assert.NotNull(bookRequest);
            Assert.Equal(bookRequestDto.Title, bookRequest.Title);
            Assert.Equal(bookRequestDto.DateRequested, bookRequest.DateRequested);
            Assert.Equal(bookRequestDto.AcceptedReturnDate, bookRequest.AcceptedReturnDate);
            Assert.Equal(bookRequestDto.ClientId, bookRequest.ClientId);
            Assert.Equal(bookRequestDto.BookId, bookRequest.BookId);
        }
    }
}