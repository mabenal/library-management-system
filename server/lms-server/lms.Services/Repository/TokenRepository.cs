using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace lms.Services.Repository
{
    public class TokenRepository : ITokenRepository
    {
        public TokenRepository(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }
        public IConfiguration configuration { get; }

        private readonly UserManager<ApplicationUser> userManager;

        public async Task<string> CreateJWTTokenAsync(ApplicationUser user)
        {
            // Retrieve configuration values
            var key = configuration["Jwt:Key"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("JWT configuration is missing.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("ClientId", user.Email.ToString())
            };


            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    
            try
            {
                var userToken = new IdentityUserToken<string>
                {
                    UserId = user.Id.ToString(),
                    LoginProvider = "JWT",
                    Name = "AccessToken",
                    Value = tokenString
                };

                await userManager.SetAuthenticationTokenAsync(user, userToken.LoginProvider, userToken.Name, userToken.Value);
            }
            catch (Exception ex)
            {
                // Log and handle the exception
                throw new InvalidOperationException("Failed to store the JWT token.", ex);
            }

            return tokenString;
        }
    }
}

