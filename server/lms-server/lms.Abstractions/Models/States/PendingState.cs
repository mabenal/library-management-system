using lms.Abstractions.Data;
using lms.Abstractions.Exceptions;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.States;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.States
{
    public class PendingState : IBookRequestState
    {
        public async Task Approve(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            bookRequest.Status = "Approved";
            bookRequest.DateApproved = DateTime.Now;
            bookRequest.SetState(new ApprovedState());
            dbContext.BookRequests.Update(bookRequest);
            await dbContext.SaveChangesAsync();
        }

        public async Task Cancel(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            bookRequest.Status = "Cancelled";
            bookRequest.SetState(new CancelledState());
            dbContext.BookRequests.Update(bookRequest);
            await dbContext.SaveChangesAsync();
        }

        public Task Pending(BookRequest bookRequest)
        {
            throw new GlobalException("The book request is already pending.");
        }

        public Task Return(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            throw new GlobalException("Pending book requests cannot be returned.");
        }

        public Task MarkAsOverdue(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            throw new GlobalException("Pending book requests cannot be marked as overdue.");
        }
    }
}