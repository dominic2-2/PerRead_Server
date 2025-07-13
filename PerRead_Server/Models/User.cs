using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("email")]
        public string Email { get; set; }

        [FirestoreProperty("password_hash")]
        public string PasswordHash { get; set; }

        [FirestoreProperty("full_name")]
        public string FullName { get; set; }

        [FirestoreProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [FirestoreProperty("role")]
        public string Role { get; set; } // "User", "Staff", "Admin"

        [FirestoreProperty("is_active")]
        public bool IsActive { get; set; }

        [FirestoreProperty("subscription_plan")]
        public string SubscriptionPlan { get; set; } // "Free", "Basic", "Premium"

        [FirestoreProperty("subscription_expiry")]
        public DateTime? SubscriptionExpiry { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
