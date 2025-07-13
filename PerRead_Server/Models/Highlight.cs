using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class Highlight
    {
        [FirestoreProperty("page_number")]
        public int PageNumber { get; set; }

        [FirestoreProperty("selected_text")]
        public string SelectedText { get; set; }

        [FirestoreProperty("color")]
        public string Color { get; set; }

        [FirestoreProperty("note")]
        public string Note { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
