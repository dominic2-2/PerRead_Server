using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class SupportTicket
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("subject")]
        public string Subject { get; set; }

        [FirestoreProperty("message")]
        public string Message { get; set; }

        [FirestoreProperty("status")]
        public string Status { get; set; } // "Open", "In Progress", "Resolved", "Closed"

        [FirestoreProperty("priority")]
        public string Priority { get; set; } // "Low", "Medium", "High"

        [FirestoreProperty("assigned_to")]
        public string AssignedTo { get; set; }

        [FirestoreProperty("responses")]
        public List<TicketResponse> Responses { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
