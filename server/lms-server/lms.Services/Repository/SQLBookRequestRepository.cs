using lms.Abstractions.Data;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using lms.Abstractions.Exceptions;

namespace lms.Services.Repository
{
    public class SQLBookRequestRepository : IBookRequestRepository
    {
        private readonly ILmsDbContext dbContext;

        public SQLBookRequestRepository(ILmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<BookRequest> AddNewRequest(BookRequest bookRequest)
        {
            var alreadyRequested = await dbContext.BookRequests.FirstOrDefaultAsync(br => br.BookId == bookRequest.BookId && br.ClientId == bookRequest.ClientId);
            var bookstatus = await dbContext.BookRequests.AnyAsync(bs => bs.BookId == bookRequest.BookId && (bs.Status == "Available" || bs.Status == "Pending" || bs.Status == "Approved"));
            if (alreadyRequested != null && bookstatus)
            {
                throw new GlobalException("You've already requested this book, check your book request history");
            }

            var bookEntity = await dbContext.Books.FindAsync(bookRequest.BookId);

            if (bookEntity == null)
            {
                throw new GlobalException("The book you are trying to request does not exist");
            }
            if (bookEntity.NumberOfCopies == 0)
            {
                throw new GlobalException("The book has no available copies at the moment, please try again later");
            }

            var bookTitle = bookEntity.Title;

            bookRequest.Title = bookTitle;
            bookRequest.Status = "Pending";
            bookRequest.DateRequested = DateTime.Now;
            bookRequest.DateReturned = null;
            bookRequest.DateApproved = null;

            await dbContext.BookRequests.AddAsync(bookRequest);
            await dbContext.SaveChangesAsync();

            return bookRequest;
        }

        public async Task<List<BookRequest>> GetAllBookRequestsAsync()
        {
            return await dbContext.BookRequests.ToListAsync();
        }

        public async Task<List<BookRequest>> GetBookRequestsByClientId(Guid clientId)
        {
            return await dbContext.BookRequests.Where(br => br.ClientId == clientId).ToListAsync();
        }

        public async Task<BookRequest> ApproveRequest(Guid clientId, Guid bookId)
        {
            var bookRequestToApprove = await dbContext.BookRequests.SingleOrDefaultAsync(br => br.ClientId == clientId && br.BookId == bookId);

            if (bookRequestToApprove == null || bookRequestToApprove.Status != "Pending")
            {
                throw new GlobalException("Book request is not in pending state and cannot be approved.");
            }

            bookRequestToApprove.DateApproved = DateTime.Now;
            bookRequestToApprove.AcceptedReturnDate = DateTime.Now.AddDays(15);
            bookRequestToApprove?.Approve(dbContext);
            await dbContext.SaveChangesAsync();
            return bookRequestToApprove;
        }

        public async Task<BookRequest> CancelRequest(Guid clientId, Guid bookId)
        {
            var bookRequestToCancel = await dbContext.BookRequests.SingleOrDefaultAsync(br => br.ClientId == clientId && br.BookId == bookId);
            var bookstatus = await dbContext.BookRequests.AnyAsync(bs => bs.BookId == bookId && bs.Status == "Pending");

            if (bookRequestToCancel == null && !bookstatus)
            {
                throw new GlobalException("Book request not found.");
            }

            await bookRequestToCancel.Cancel(dbContext);
            return bookRequestToCancel;
        }

        public async Task<BookRequest> ReturnRequest(Guid clientId, Guid bookId)
        {
            var bookRequestToReturn = await dbContext.BookRequests.SingleOrDefaultAsync(br => br.ClientId == clientId && br.BookId == bookId);
            var bookstatus = await dbContext.BookRequests.AnyAsync(bs => bs.BookId == bookId && bs.Status == "Approved");

            if (bookRequestToReturn == null && !bookstatus)
            {
                throw new GlobalException("Approved book request not found.");
            }

            bookRequestToReturn.DateReturned = DateTime.Now;
            await bookRequestToReturn.Return(dbContext);
            await dbContext.SaveChangesAsync();
            return bookRequestToReturn;
        }

        public async Task<BookRequest> OverdueRequest(Guid clientId, Guid bookId)
        {
            var bookRequestToOverdue = await dbContext.BookRequests.SingleOrDefaultAsync(br => br.ClientId == clientId && br.BookId == bookId);

            if (bookRequestToOverdue == null)
            {
                throw new GlobalException("Book request is not overdue.");
            }

            await bookRequestToOverdue.MarkAsOverdue(dbContext);
            await dbContext.SaveChangesAsync();
            return bookRequestToOverdue;
        }
    }
}