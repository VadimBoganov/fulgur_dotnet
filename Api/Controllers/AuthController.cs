using Api.Models;
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
        private readonly AdminContext _adminContext = adminContext;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost]
        public async Task<IResult> Login(User loginData)
        {
            var user = await _adminContext.Users.FirstOrDefaultAsync(u => u.Email == loginData.Email && u.Password == loginData.Password);

            if (user is null) return Results.Unauthorized();

            var claims = new List<Claim> { new(ClaimTypes.Name, user.Email) };

#pragma warning disable CS8604 
            var jwt = new JwtSecurityToken(
                issuer: _configuration["Auth:Issuer"],
                audience: _configuration["Auth:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(360)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:SecretKey"])), SecurityAlgorithms.HmacSha256));
#pragma warning restore CS8604 

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = user.Email
            };

            return Results.Json(response);
        }
    }
}
