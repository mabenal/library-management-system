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
            if (alreadyRequested != null)
            {
                throw new GoblalException("You've already requested this book, check your book request history");
            }
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

    }
}
