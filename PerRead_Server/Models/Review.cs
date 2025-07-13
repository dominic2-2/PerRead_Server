using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class Review
    {
        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("comment")]
        public string Comment { get; set; }

        [FirestoreProperty("rating")]
        public int Rating { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
