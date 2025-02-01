using lms.Abstractions.Data;
using lms.Abstractions.Interfaces;

namespace lms.Abstractions.Models.States
{
    public interface IBookRequestState
    {
        Task Approve(BookRequest bookRequest, ILmsDbContext dbContext);
        Task Cancel(BookRequest bookRequest, ILmsDbContext dbContext);
        Task Pending(BookRequest bookRequest);
        Task Return(BookRequest bookRequest, ILmsDbContext dbContext);
        Task MarkAsOverdue(BookRequest bookRequest, ILmsDbContext dbContext);
    }
}