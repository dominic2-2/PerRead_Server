using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class Author
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string? Name { get; set; }

        [FirestoreProperty("bio")]
        public string? Bio { get; set; }

        [FirestoreProperty("email")]
        public string? Email { get; set; }

        [FirestoreProperty("birth_year")]
        public int? BirthYear { get; set; }

        [FirestoreProperty("death_year")]
        public int? DeathYear { get; set; }

        [FirestoreProperty("created_at")]
        public Timestamp? CreatedAt { get; set; }

        [FirestoreProperty("updated_at")]
        public Timestamp? UpdatedAt { get; set; }

    }
}
