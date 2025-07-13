using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class Category
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}