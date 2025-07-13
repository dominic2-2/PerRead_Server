using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class ImportedBook
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("title")]
        public string Title { get; set; }

        [FirestoreProperty("author")]
        public string Author { get; set; }

        [FirestoreProperty("file_url")]
        public string FileUrl { get; set; }

        [FirestoreProperty("file_name")]
        public string FileName { get; set; }

        [FirestoreProperty("file_size")]
        public long FileSize { get; set; }

        [FirestoreProperty("cover_url")]
        public string CoverUrl { get; set; }

        [FirestoreProperty("pages")]
        public int Pages { get; set; }

        [FirestoreProperty("imported_at")]
        public DateTime ImportedAt { get; set; }

        [FirestoreProperty("last_read_at")]
        public DateTime? LastReadAt { get; set; }
    }
}
