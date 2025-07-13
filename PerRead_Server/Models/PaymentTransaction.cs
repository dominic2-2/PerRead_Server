using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class PaymentTransaction
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("book_id")]
        public string BookId { get; set; }

        [FirestoreProperty("subscription_package_id")]
        public string SubscriptionPackageId { get; set; }

        [FirestoreProperty("amount")]
        public decimal Amount { get; set; }

        [FirestoreProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [FirestoreProperty("transaction_type")]
        public string TransactionType { get; set; } // "Book", "Subscription"

        [FirestoreProperty("status")]
        public string Status { get; set; } // "Pending", "Success", "Failed", "Refunded"

        [FirestoreProperty("payment_gateway_id")]
        public string PaymentGatewayId { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
