using Google.Cloud.Firestore;

namespace PerRead_Server.Models
{
    [FirestoreData]
    public class TicketResponse
    {
        [FirestoreProperty("responder_id")]
        public string ResponderId { get; set; }

        [FirestoreProperty("message")]
        public string Message { get; set; }

        [FirestoreProperty("is_staff_response")]
        public bool IsStaffResponse { get; set; }

        [FirestoreProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
