using lms.Abstractions.Data;
using lms.Abstractions.Exceptions;
using lms.Abstractions.Models;
using lms.Abstractions.Models.States;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.States
{
    public class ApprovedState : IBookRequestState
    {
        public Task Approve(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("The book request is already approved.");
        }

        public Task Cancel(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("Approved book requests cannot be canceled.");
        }

        public Task Pending(BookRequest bookRequest)
        {
            throw new GlobalException("Approved book requests cannot be pending.");
        }

        public async Task Return(BookRequest bookRequest, LmsDbContext dbContext)
        {
            bookRequest.Status = "Returned";
            bookRequest.DateReturned = DateTime.Now;
            bookRequest.SetState(new ReturnState());
            dbContext.BookRequests.Update(bookRequest);
            await dbContext.SaveChangesAsync();
        }

        public async Task MarkAsOverdue(BookRequest bookRequest, LmsDbContext dbContext)
        {
            bookRequest.Status = "Overdue";
            bookRequest.SetState(new OverdueState());
            dbContext.BookRequests.Update(bookRequest);
            await dbContext.SaveChangesAsync();
        }
    }
}