using lms.Abstractions.Data;
using lms.Abstractions.Exceptions;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.States;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.States
{
    public class CancelledState : IBookRequestState
    {
        public Task Approve(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            throw new GlobalException("Cancelled book requests cannot be approved.");
        }

        public Task Cancel(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            throw new GlobalException("The book request is already canceled.");
        }

        public Task Pending(BookRequest bookRequest)
        {
            throw new GlobalException("Cancelled book requests cannot be pending.");
        }

        public Task Return(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            throw new GlobalException("Cancelled book requests cannot be returned.");
        }

        public Task MarkAsOverdue(BookRequest bookRequest, ILmsDbContext dbContext)
        {
            throw new GlobalException("Cancelled book requests cannot be marked as overdue.");
        }
    }
}