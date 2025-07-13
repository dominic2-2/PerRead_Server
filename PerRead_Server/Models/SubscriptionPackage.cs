using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class SubscriptionPackage
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("price")]
        public decimal Price { get; set; }

        [FirestoreProperty("duration_days")]
        public int DurationDays { get; set; }

        [FirestoreProperty("features")]
        public List<string> Features { get; set; }

        [FirestoreProperty("max_books")]
        public int MaxBooks { get; set; }

        [FirestoreProperty("is_active")]
        public bool IsActive { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
