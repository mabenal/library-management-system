using lms.Abstractions.Interfaces;
using lms.Services.Repository;
using lms.Abstractions.Models.States;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lms_server.Repository;

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

                // Register state classes
                services.AddScoped<PendingState>();
                services.AddScoped<ApprovedState>();
                services.AddScoped<CancelledState>();
                services.AddScoped<ReturnState>();
                services.AddScoped<OverdueState>();
        }
    }
}