using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AdminContext adminContext, IConfiguration configuration) : ControllerBase
    {
        private const int DefaultAccessTokenMinutes = 60;
        private const string AuthCookieName = "access_token";

        private readonly AdminContext _adminContext = adminContext;
        private readonly IConfiguration _configuration = configuration;
        private static readonly PasswordHasher<User> _passwordHasher = new();

        [HttpPost]
        public async Task<IResult> Login(User loginData)
        {
            var user = await _adminContext.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email);

            if (user is null) return Results.Unauthorized();

            if (!await VerifyPasswordAsync(user, loginData.Password)) return Results.Unauthorized();

            var secretKey = _configuration["Auth:SecretKey"]
                ?? throw new InvalidOperationException("Auth:SecretKey is not configured.");

            var minutes = int.TryParse(_configuration["Auth:AccessTokenMinutes"], out var configured)
                ? configured
                : DefaultAccessTokenMinutes;

            var claims = new List<Claim> { new(ClaimTypes.Name, user.Email) };

            var jwt = new JwtSecurityToken(
                issuer: _configuration["Auth:Issuer"],
                audience: _configuration["Auth:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            Response.Cookies.Append(AuthCookieName, encodedJwt, BuildCookieOptions(DateTimeOffset.UtcNow.AddMinutes(minutes)));

            return Results.Json(new { username = user.Email });
        }

        [HttpPost("logout")]
        public IResult Logout()
        {
            Response.Cookies.Delete(AuthCookieName, BuildCookieOptions(null));
            return Results.Ok();
        }

        [HttpGet("me")]
        [Authorize]
        public IResult Me() => Results.Json(new { username = User.Identity?.Name });

        private CookieOptions BuildCookieOptions(DateTimeOffset? expires)
        {
            var secure = !bool.TryParse(_configuration["Auth:CookieSecure"], out var configured) || configured;

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = expires
            };
        }

        // Verifies the supplied password against the stored hash. Passwords created
        // before hashing was introduced are stored in plain text; on a successful
        // legacy match the record is transparently upgraded to a hash.
        private async Task<bool> VerifyPasswordAsync(User user, string suppliedPassword)
        {
            PasswordVerificationResult result;

            try
            {
                result = _passwordHasher.VerifyHashedPassword(user, user.Password, suppliedPassword);
            }
            catch (FormatException)
            {
                result = PasswordVerificationResult.Failed;
            }

            if (result == PasswordVerificationResult.Success)
                return true;

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _passwordHasher.HashPassword(user, suppliedPassword);
                await _adminContext.SaveChangesAsync();
                return true;
            }

            if (user.Password == suppliedPassword)
            {
                user.Password = _passwordHasher.HashPassword(user, suppliedPassword);
                await _adminContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
