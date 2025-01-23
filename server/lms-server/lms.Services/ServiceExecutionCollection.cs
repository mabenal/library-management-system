using lms.Abstractions.Interfaces;
using lms.Services.Repository;
using lms_server.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Services
{
    public static class ServiceExecutionCollection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<BookImportService>()
                .AddScoped<IBooksRepository, SQLBooksRepository>()
                .AddScoped<IClientRepository, SQLClientRepository>()
                .AddScoped<IBookRequestRepository, SQLBookRequestRepository>()
                .AddScoped<ITokenRepository, TokenRepository>();
            // Add other services here
        }
    }
}
