using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using PerRead_Server.DTOs;
using PerRead_Server.Models;

namespace PerRead_Server.Services
{
    public class UserService
    {
        private readonly FirestoreDb _db;

        public UserService(FirestoreService firestore)
        {
            _db = firestore.Db;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var snapshot = await _db.Collection("users").GetSnapshotAsync();
            return snapshot.Documents.Select(d =>
            {
                var u = d.ConvertTo<User>();
                u.Id = d.Id;
                return u;
            }).ToList();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var doc = await _db.Collection("users").Document(id).GetSnapshotAsync();
            if (doc.Exists)
            {
                var user = doc.ConvertTo<User>();
                user.Id = doc.Id;
                return user;
            }
            return null;
        }

        public async Task<User> CreateAsync(UserDTO userDto)
        {
            var existingUser = await _db.Collection("users")
                                        .WhereEqualTo("email", userDto.Email)
                                        .Limit(1)
                                        .GetSnapshotAsync();

            if (existingUser.Count > 0)
                throw new InvalidOperationException("Email already exists.");

            var user = new User
            {
                Email = userDto.Email ?? "",
                FullName = userDto.FullName ?? "",
                AvatarUrl = userDto.AvatarUrl ?? "",
                Role = userDto.Role ?? "User",
                IsActive = userDto.IsActive ?? true,
                SubscriptionPlan = userDto.SubscriptionPlan ?? "Free",
                SubscriptionExpiry = userDto.SubscriptionExpiry,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (string.IsNullOrWhiteSpace(userDto.PasswordHash))
                throw new ArgumentException("Password is required.");

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, userDto.PasswordHash);

            var added = await _db.Collection("users").AddAsync(user);
            user.Id = added.Id;
            return user;
        }

        public async Task<User?> UpdateAsync(string id, User user)
        {
            var docRef = _db.Collection("users").Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return null;

            var existing = snapshot.ConvertTo<User>();
            if (existing == null) return null;
            
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = existing.PasswordHash;
            }

            user.Id = id;
            user.CreatedAt = existing.CreatedAt;
            user.UpdatedAt = DateTime.UtcNow;

            await docRef.SetAsync(user, SetOptions.Overwrite);
            return user;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var docRef = _db.Collection("users").Document(id);
            var doc = await docRef.GetSnapshotAsync();
            if (!doc.Exists)
                return false;

            var updates = new Dictionary<string, object>
            {
                { "is_active", false },
                { "updated_at", DateTime.UtcNow }
            };
            await docRef.UpdateAsync(updates);
            return true;
        }
    }
}
