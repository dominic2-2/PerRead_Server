using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class ReadingProgress
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("book_id")]
        public string BookId { get; set; }

        [FirestoreProperty("current_page")]
        public int CurrentPage { get; set; }

        [FirestoreProperty("total_pages")]
        public int TotalPages { get; set; }

        [FirestoreProperty("progress_percentage")]
        public double ProgressPercentage { get; set; }

        [FirestoreProperty("bookmarks")]
        public List<Bookmark> Bookmarks { get; set; }

        [FirestoreProperty("highlights")]
        public List<Highlight> Highlights { get; set; }

        [FirestoreProperty("reading_time_minutes")]
        public int ReadingTimeMinutes { get; set; }

        [FirestoreProperty("last_read_at")]
        public DateTime LastReadAt { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
