using Google.Cloud.Firestore;

namespace PerRead_Server.Services
{
    public class FirestoreService
    {
        public FirestoreDb Db { get; }

        public FirestoreService(IConfiguration config)
        {
            var keyPath = Path.Combine(AppContext.BaseDirectory, "firebase-key.json");

            if (!File.Exists(keyPath))
                throw new FileNotFoundException($"Firebase key not found at {keyPath}");

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", keyPath);
            Db = FirestoreDb.Create("perread-aa3dd");
        }
    }
}
