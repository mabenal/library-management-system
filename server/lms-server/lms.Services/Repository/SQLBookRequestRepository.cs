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
    public class SQLBookRequestRepository: IBookRequestRepository
    {
        private readonly LmsDbContext dbContext;

        public SQLBookRequestRepository(LmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<BookRequest> AddNewRequest(BookRequest bookRequest)
        {
            var alreadyRequested = await dbContext.BookRequests.FirstOrDefaultAsync(br => br.BookId == bookRequest.BookId && br.ClientId == bookRequest.ClientId);
            var bookstatus = await dbContext.BookRequests.AnyAsync(bs => bs.BookId == bookRequest.BookId && (bs.Status == "Available" || bs.Status == "Pending" || bs.Status == "Approved"));
            if (alreadyRequested != null && bookstatus)
            {
                throw new GoblalException("You've already requested this book, check your book request history");
            }

            var bookEntity= await dbContext.Books.FindAsync(bookRequest.BookId);

            if(bookEntity == null)
            {
                throw new GoblalException("The book you are trying to request does not exist");
            }
            if (bookEntity.NumberOfCopies == 0)
            {
                throw new GoblalException("The book has no available copies at the moment, please try again later");
            }

            var bookTitle = bookEntity.Title;

            bookRequest.Title = bookTitle;  
            bookEntity.NumberOfCopies--;
            bookRequest.Status = "Pending";
            bookRequest.DateRequested  = DateTime.Now;

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

        public async Task<BookRequest> ApproveRequest(Guid clientId, Guid bookId, BookRequest bookRequest)
        {
            var bookRequestToApprove = await dbContext.BookRequests.SingleOrDefaultAsync(br => br.ClientId == clientId && br.BookId == bookId);
            var bookstatus = await dbContext.BookRequests.AnyAsync(bs => bs.BookId == bookRequest.BookId && bs.ClientId == bookRequest.BookId && bs.Status == "Pending");

            if (bookRequestToApprove == null)
            {
                return null;
            }
            if(!bookstatus)
            {
                throw new GoblalException("Only books with a Pending status can be approved");

            }

            bookRequestToApprove.Status = "Approved";
            bookRequestToApprove.DateApproved = DateTime.Now;

            await dbContext.SaveChangesAsync();
            return bookRequestToApprove;
        }

        public async Task<BookRequest> CancelResquest(Guid clientId, Guid bookId, BookRequest bookRequest)
        {
            var bookRequestToCancel = await dbContext.BookRequests.SingleOrDefaultAsync(br => br.ClientId == clientId && br.BookId == bookId);
            var bookstatus = await dbContext.BookRequests.AnyAsync(bs => bs.BookId == bookRequest.BookId && bs.ClientId == bookRequest.BookId && bs.Status == "Pending");


            if (bookRequestToCancel == null)
            {
                return null;
            }
            if (!bookstatus)
            {
                throw new GoblalException("Only books with a Pending status can be canceled");

            }

            bookRequestToCancel.Status = "Cancelled";

            await dbContext.SaveChangesAsync();
            return bookRequestToCancel;
        }
    }
}
