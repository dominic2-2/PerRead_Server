using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class Book
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("title")]
        public string Title { get; set; }

        [FirestoreProperty("author_ids")]
        public List<string> AuthorIds { get; set; }

        [FirestoreProperty("publisher_id")]
        public string PublisherId { get; set; }

        [FirestoreProperty("category_ids")]
        public List<string> CategoryIds { get; set; }

        [FirestoreProperty("language")]
        public string Language { get; set; }

        [FirestoreProperty("edition")]
        public string Edition { get; set; }

        [FirestoreProperty("isbn")]
        public string ISBN { get; set; }

        [FirestoreProperty("pages")]
        public int Pages { get; set; }

        [FirestoreProperty("summary")]
        public string Summary { get; set; }

        [FirestoreProperty("cover_url")]
        public string CoverUrl { get; set; }

        [FirestoreProperty("file_url")]
        public string FileUrl { get; set; }

        [FirestoreProperty("file_size")]
        public long FileSize { get; set; }

        [FirestoreProperty("price")]
        public decimal Price { get; set; }

        [FirestoreProperty("availability")]
        public bool Availability { get; set; }

        [FirestoreProperty("tags")]
        public List<string> Tags { get; set; }

        [FirestoreProperty("average_rating")]
        public double AverageRating { get; set; }

        [FirestoreProperty("total_reviews")]
        public int TotalReviews { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
