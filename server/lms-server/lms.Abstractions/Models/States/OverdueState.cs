using lms.Abstractions.Data;
using lms.Abstractions.Exceptions;
using lms.Abstractions.Models;
using lms.Abstractions.Models.States;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.States
{
    public class OverdueState : IBookRequestState
    {
        public Task Approve(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("Overdue book requests cannot be approved.");
        }

        public Task Cancel(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("Overdue book requests cannot be canceled.");
        }

        public Task Pending(BookRequest bookRequest)
        {
            throw new GlobalException("Overdue book requests cannot be pending.");
        }

        public async Task Return(BookRequest bookRequest, LmsDbContext dbContext)
        {
            bookRequest.Status = "Returned";
            bookRequest.DateReturned = DateTime.Now;
            bookRequest.SetState(new ReturnState());
            dbContext.BookRequests.Update(bookRequest);
            await dbContext.SaveChangesAsync();
        }

        public Task MarkAsOverdue(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("The book request is already overdue.");
        }
    }
}