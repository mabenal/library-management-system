using System.ComponentModel.DataAnnotations.Schema;
using lms.Abstractions.Data;
using lms.Abstractions.Models.States;
using Microsoft.EntityFrameworkCore;

namespace lms.Abstractions.Models
{
    public class BookRequest
    {
        public Guid Id { get; set; }
        public required string Status { get; set; }
        public required string Title { get; set; }
        public required DateTime DateRequested { get; set; }
        public required DateTime AcceptedReturnDate { get; set; }
        public DateTime? DateApproved { get; set; }
        public DateTime? DateReturned { get; set; }

        [ForeignKey("Client")]
        public Guid ClientId { get; set; }
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public required Client Client { get; set; }
        public required Book Book { get; set; }
        private IBookRequestState _state;

        private BookRequest()
        {
            _state = new PendingState();
        }

        public static BookRequest CreatePendingRequest(string title, DateTime dateRequested, DateTime acceptedReturnDate, Client client, Book book)
        {
            var bookRequest = new BookRequest
            {
                Status = "Pending",
                Title = title,
                DateRequested = dateRequested,
                AcceptedReturnDate = acceptedReturnDate,
                Client = client,
                Book = book
            };
            bookRequest.SetState(new PendingState());
            return bookRequest;
        }

        public void SetState(IBookRequestState state)
        {
            _state = state;
        }

        public async Task Approve(LmsDbContext dbContext)
        {
            await _state.Approve(this, dbContext);
            dbContext.BookRequests.Update(this);
            await dbContext.SaveChangesAsync();
        }

        public async Task Cancel(LmsDbContext dbContext)
        {
            await _state.Cancel(this, dbContext);
            dbContext.BookRequests.Update(this);
            await dbContext.SaveChangesAsync();
        }

        public async Task Return(LmsDbContext dbContext)
        {
            await _state.Return(this, dbContext);
            dbContext.BookRequests.Update(this);
            await dbContext.SaveChangesAsync();
        }

        public async Task MarkAsOverdue(LmsDbContext dbContext)
        {
            await _state.MarkAsOverdue(this, dbContext);
            dbContext.BookRequests.Update(this);
            await dbContext.SaveChangesAsync();
        }
    }
}