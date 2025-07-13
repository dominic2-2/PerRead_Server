using Microsoft.AspNetCore.Mvc;
using PerRead_Server.Services;
using System.ComponentModel.DataAnnotations;

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
            var user = await _auth.AuthenticateAsync(request.Email, request.PasswordHash);

            if (user == null)
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
