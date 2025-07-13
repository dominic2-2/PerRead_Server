namespace PerRead_Server.DTOs
{
    public class UserDTO
    {
        public string? Id { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? FullName { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Role { get; set; } // "User", "Staff", "Admin"

        public bool? IsActive { get; set; }

        public string? SubscriptionPlan { get; set; } // "Free", "Basic", "Premium"

        public DateTime? SubscriptionExpiry { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
