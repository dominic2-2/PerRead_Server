using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class BookCollection
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("description")]
        public string Description { get; set; }

        [FirestoreProperty("book_ids")]
        public List<string> BookIds { get; set; }

        [FirestoreProperty("is_public")]
        public bool IsPublic { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
