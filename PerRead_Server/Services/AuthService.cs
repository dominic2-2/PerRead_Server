using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using PerRead_Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerRead_Server.Services
{
    public class AuthService
    {
        private readonly FirestoreDb _db;
        private readonly IConfiguration _config;

        public AuthService(FirestoreService firestore, IConfiguration config)
        {
            _db = firestore.Db;
            _config = config;
        }

        public async Task<User?> AuthenticateAsync(string email, string passwordHash)
        {
            var query = await _db.Collection("users")
                                 .WhereEqualTo("email", email)
                                 .WhereEqualTo("password_hash", passwordHash)
                                 .WhereEqualTo("is_active", true)
                                 .GetSnapshotAsync();

            var doc = query.Documents.FirstOrDefault();
            return doc?.ConvertTo<User>();
        }

        public string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id),
                new Claim("email", user.Email),
                new Claim("full_name", user.FullName ?? ""),
                new Claim("role", user.Role ?? "User"),
                new Claim("avatar", user.AvatarUrl ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:ExpireDays"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
