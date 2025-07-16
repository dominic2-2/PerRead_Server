using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PerRead_Server.Services;
using System.ComponentModel.DataAnnotations;
using PerRead_Server.Models;

namespace PerRead_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        public record LoginRequest([Required] string Email, [Required] string PasswordHash);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _auth.GetUserByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
                return Unauthorized(new { message = "Invalid credentials or inactive user." });

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.PasswordHash);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid credentials or inactive user." });

            var token = _auth.GenerateJwt(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.Role,
                    user.AvatarUrl
                }
            });
        }
    }
}
