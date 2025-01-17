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
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
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

// Add Identity services
builder.Services.AddIdentity<Client, IdentityRole<Guid>>() // Assuming 'Client' is your user model
    .AddEntityFrameworkStores<LmsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Ensure roles and admin user are created
await CreateRolesAsync(app);

app.Run();


static async Task CreateRolesAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Client>>();

    string[] roleNames = { "Client", "Library Manager", "Admin" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
    if (adminUser == null)
    {
        var user = new Client { UserName = "admin@example.com", Email = "admin@example.com" };
        var result = await userManager.CreateAsync(user, "Test@123");

        if (result.Succeeded)
        {
           
        }
    }
}
}
