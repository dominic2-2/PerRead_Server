using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PerRead_Server.DTOs;
using PerRead_Server.Models;
using PerRead_Server.Services;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerRead_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly UserService _userService;
        private readonly EmailService _emailService;
        private readonly OTPService _otpService;

        public AuthController(AuthService auth, UserService userService, EmailService emailService, OTPService otpService)
        {
            _auth = auth;
            _userService = userService;
            _emailService = emailService;
            _otpService = otpService;
        }

        public record LoginRequest([Required] string Email, [Required] string Password);
        public record SignupRequest([Required] string Email, [Required] string Password, [Required] string RePassword);
        public record ForgotPasswordRequest([Required] string Email);
        public record ResetPasswordRequest([Required] string Email, [Required] string OTP, [Required] string NewPassword);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _auth.GetUserByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
                return Unauthorized(new { message = "Invalid credentials or inactive user." });

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

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

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (request.Password != request.RePassword)
            {
                return BadRequest(new { message = "Password and RePassword do not match." });
            }

            var existingUser = await _userService.GetUsersAsync();
            if (existingUser.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already exists." });
            }

            var userDto = new UserDTO
            {
                Email = request.Email,
                PasswordHash = request.Password,
                Role = "User"
            };

            try
            {
                var newUser = await _userService.CreateAsync(userDto);
                return Ok(new { message = "Create account successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userService.GetUsersAsync();
            var existingUser = user.FirstOrDefault(u => u.Email == request.Email);

            if (existingUser == null)
            {
                return NotFound(new { message = "Email not found." });
            }

            var otp = new Random().Next(100000, 1000000).ToString("D6");

            var subject = "Password Reset OTP";
            var body = $"<p>Your OTP for password reset is: <strong>{otp}</strong></p>";

            try
            {
                await _emailService.SendEmailAsync(request.Email, subject, body);

                _otpService.StoreOtp(request.Email, otp);

                return Ok(new { message = "OTP sent to your email." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to send OTP: " + ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userService.GetUsersAsync();
            var existingUser = user.FirstOrDefault(u => u.Email == request.Email);

            if (existingUser == null)
            {
                return NotFound(new { message = "Email not found." });
            }

            if (!_otpService.ValidateOtp(request.Email, request.OTP))
            {
                return BadRequest(new { message = "Invalid or expired OTP." });
            }

            var hasher = new PasswordHasher<User>();
            var hashedPassword = hasher.HashPassword(existingUser, request.NewPassword);
            existingUser.PasswordHash = hashedPassword;

            await _userService.UpdateAsync(existingUser.Id, existingUser);

            _otpService.RemoveOtp(request.Email);

            return Ok(new { message = "Password reset successfully." });
        }
    }
}
