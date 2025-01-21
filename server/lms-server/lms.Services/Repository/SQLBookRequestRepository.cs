using lms.Abstractions.Data;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lms.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            await dbContext.BookRequests.AddAsync(bookRequest);
            await dbContext.SaveChangesAsync();

            return bookRequest;
        }

        public async Task<List<BookRequest>> GetAllBookRequestsAsync()
        {
            return await dbContext.BookRequests.ToListAsync();
        }


    }
}
