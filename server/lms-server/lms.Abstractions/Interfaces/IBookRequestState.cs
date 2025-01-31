using lms.Abstractions.Data;

namespace lms.Abstractions.Models.States
{
    public interface IBookRequestState
    {
        Task Approve(BookRequest bookRequest, LmsDbContext dbContext);
        Task Cancel(BookRequest bookRequest, LmsDbContext dbContext);
        Task Pending(BookRequest bookRequest);
        Task Return(BookRequest bookRequest, LmsDbContext dbContext);
        Task MarkAsOverdue(BookRequest bookRequest, LmsDbContext dbContext);
    }
}