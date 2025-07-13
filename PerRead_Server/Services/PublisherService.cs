using Google.Cloud.Firestore;
using PerRead_Server.DTOs;
using PerRead_Server.DTOs.PerRead_Server.DTOs;
using PerRead_Server.Models;
using System.Globalization;

namespace PerRead_Server.Services
{
    public class PublisherService
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _publishers;

        public PublisherService(FirestoreService firestore)
        {
            _db = firestore.Db;
            _publishers = _db.Collection("publishers");
        }

        public async Task<List<Publisher>> GetAllAsync()
        {
            var snapshot = await _publishers.GetSnapshotAsync();
            return snapshot.Documents.Select(doc =>
            {
                var pub = doc.ConvertTo<Publisher>();
                pub.Id = doc.Id;
                return pub;
            }).ToList();
        }

        public async Task<Publisher?> GetByIdAsync(string id)
        {
            var doc = await _publishers.Document(id).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var pub = doc.ConvertTo<Publisher>();
            pub.Id = doc.Id;
            return pub;
        }

        public async Task<Publisher?> CreateAsync(PublisherCreateDTO dto)
        {
            var normalizedName = NormalizeName(dto.Name);

            var exists = await _publishers
                .WhereEqualTo("name", normalizedName)
                .GetSnapshotAsync();

            if (exists.Count > 0)
                return null;

            var pub = new Publisher
            {
                Name = normalizedName,
                Address = dto.Address,
                Website = dto.Website,
                Email = dto.Email,
                Phone = dto.Phone
            };

            var added = await _publishers.AddAsync(pub);
            pub.Id = added.Id;
            return pub;
        }

        public async Task<Publisher?> UpdateAsync(string id, PublisherUpdateDTO dto)
        {
            var doc = _publishers.Document(id);
            var snapshot = await doc.GetSnapshotAsync();
            if (!snapshot.Exists) return null;

            var normalizedName = NormalizeName(dto.Name);
            var all = await GetAllAsync();

            var duplicate = all.Any(p =>
                p.Id != id &&
                string.Equals(p.Name, normalizedName, StringComparison.OrdinalIgnoreCase));

            if (duplicate)
                return null;

            var pub = new Publisher
            {
                Name = normalizedName,
                Address = dto.Address,
                Website = dto.Website,
                Email = dto.Email,
                Phone = dto.Phone
            };

            await doc.SetAsync(pub, SetOptions.Overwrite);
            pub.Id = id;
            return pub;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var booksWithPublisher = await _db.Collection("books")
                .WhereEqualTo("publisher_id", id)
                .GetSnapshotAsync();

            if (booksWithPublisher.Count > 0)
                return false;

            var doc = _publishers.Document(id);
            var snapshot = await doc.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await doc.DeleteAsync();
            return true;
        }

        private string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.Trim().ToLower());
        }
    }
}
