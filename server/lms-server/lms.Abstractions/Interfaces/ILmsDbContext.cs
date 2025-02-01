using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Interfaces
{
    public interface ILmsDbContext
    {
        DbSet<Book> Books { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<BookRequest> BookRequests { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
