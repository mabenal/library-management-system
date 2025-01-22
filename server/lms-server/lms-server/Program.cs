using lms.Abstractions.Interfaces;
using lms.Abstractions.Data;
using lms.Abstractions.Mappings;
using lms_server.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using lms.Services;
using lms.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(); // Add this line to include controllers from lms.Peer

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "lms-server",
        Version = "v1",
        Description = "API Documentation",
    });
});

builder.Services.AddDbContext<LmsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LmsDbConnectionString"),
        b => b.MigrationsAssembly("lms-server")); // Specify the migrations assembly
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add services using ServiceExecution
builder.Services.AddServices();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Execute BookImportService on startup
using (var scope = app.Services.CreateScope())
{
    var bookImportService = scope.ServiceProvider.GetRequiredService<BookImportService>();
    var filePath = "./books.json"; // Update this path to the actual location of books.json
    await bookImportService.ImportBooksAsync(filePath);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "lms-server v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins"); // Enable CORS

app.UseAuthorization();

app.MapControllers();

app.Run();