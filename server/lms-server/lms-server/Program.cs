using lms.Abstractions.Models;
using lms_server.Data;
using lms_server.Mappings;
using lms_server.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("LmsDbConnectionString"));
});

builder.Services.AddScoped<IBooksRepository, SQLBooksRepository>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddIdentity<Client, IdentityRole<Guid>>() // to be confirmed
    .AddEntityFrameworkStores<LmsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .
          AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey= true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))


            });


var app = builder.Build();

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