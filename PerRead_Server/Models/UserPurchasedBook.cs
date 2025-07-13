using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class UserPurchasedBook
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("book_id")]
        public string BookId { get; set; }

        [FirestoreProperty("transaction_id")]
        public string TransactionId { get; set; }

        [FirestoreProperty("purchased_at")]
        public DateTime PurchasedAt { get; set; }

        [FirestoreProperty("download_count")]
        public int DownloadCount { get; set; }

        [FirestoreProperty("last_accessed")]
        public DateTime LastAccessed { get; set; }
    }
}
