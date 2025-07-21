using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using System.Text;

namespace PerRead_Server.Services
{
    public class FirestoreService
    {
        public FirestoreDb Db { get; }

        public FirestoreService(IConfiguration config)
        {
            var base64Json = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_BASE64");

            if (string.IsNullOrEmpty(base64Json))
                throw new InvalidOperationException("Firebase credential not found in environment variables.");

            var jsonBytes = Convert.FromBase64String(base64Json);
            var credential = GoogleCredential.FromJson(Encoding.UTF8.GetString(jsonBytes));

            var clientBuilder = new FirestoreClientBuilder
            {
                Credential = credential
            };
            var firestoreClient = clientBuilder.Build();

            Db = FirestoreDb.Create("perread-aa3dd", firestoreClient);
        }
    }
}
