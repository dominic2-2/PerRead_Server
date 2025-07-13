using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class Bookmark
    {
        [FirestoreProperty("page_number")]
        public int PageNumber { get; set; }

        [FirestoreProperty("note")]
        public string Note { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
