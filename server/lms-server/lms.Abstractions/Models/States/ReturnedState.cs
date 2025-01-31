using lms.Abstractions.Data;
using lms.Abstractions.Exceptions;
using lms.Abstractions.Models;
using lms.Abstractions.Models.States;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.States
{
    public class ReturnState : IBookRequestState
    {
        public Task Approve(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("Returned book requests cannot be approved.");
        }

        public Task Cancel(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("Returned book requests cannot be canceled.");
        }

        public Task Pending(BookRequest bookRequest)
        {
            throw new GlobalException("Returned book requests cannot be pending.");
        }

        public Task Return(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("The book request is already returned.");
        }

        public Task MarkAsOverdue(BookRequest bookRequest, LmsDbContext dbContext)
        {
            throw new GlobalException("Returned book requests cannot be marked as overdue.");
        }
    }
}