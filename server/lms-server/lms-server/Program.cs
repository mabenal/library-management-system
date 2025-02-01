
using lms.Abstractions.Data;
using lms.Abstractions.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using lms.Services;
using lms.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"


    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {

            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id= JwtBearerDefaults.AuthenticationScheme
                },
                Scheme="Oauth2",
                Name= JwtBearerDefaults.AuthenticationScheme,
                In= ParameterLocation.Header,
            },
             new List<string>()

        }

        });

});

// Use Singleton for ConfigurationManager
var configurationManager = lms.Abstractions.ConfigurationManager.GetInstance(builder.Configuration);
builder.Services.AddSingleton(configurationManager);

builder.Services.AddDbContext<LmsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LmsDbConnectionString"),
        b => b.MigrationsAssembly("lms-server"));
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

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
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
        ValidIssuer = configurationManager.GetJwtIssuer(),
        ValidAudience = configurationManager.GetJwtAudience(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationManager.GetJwtKey()))
    };
});
builder.Services.AddAuthorization();



builder.Services.AddHttpClient();

var app = builder.Build();

// Execute BookImportService on startup
using (var scope = app.Services.CreateScope())
{
    var bookImportService = scope.ServiceProvider.GetRequiredService<BookImportService>();
    var filePath = "./books.json"; // Update this path to the actual location of books.json
    await bookImportService.ImportBooksAsync(filePath);
}

//add the user roles to the DB and seed an admin user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "client", "librarian", "admin" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }

    var adminUser = await userManager.FindByEmailAsync("admin@lms.com");
    if (adminUser == null)
    {
        var user = new ApplicationUser { UserName = "admin@lms.com", Email = "admin@lms.com" };
        var result = await userManager.CreateAsync(user, "LmsDefaultAdmin@123");

        if (result.Succeeded)
        {
             await userManager.AddToRoleAsync(user, "admin");

        }
    }
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

