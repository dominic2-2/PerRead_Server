using Google.Cloud.Firestore;
using PerRead_Server.Models;
using PerRead_Server.DTOs;
using System.Globalization;

namespace PerRead_Server.Services
{
    public class AuthorService
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _authors;

        public AuthorService(FirestoreService firestore)
        {
            _db = firestore.Db;
            _authors = _db.Collection("authors");
        }

        public async Task<List<Author>> GetAllAsync()
        {
            var snapshot = await _authors.GetSnapshotAsync();
            return snapshot.Documents.Select(doc =>
            {
                var author = doc.ConvertTo<Author>();
                author.Id = doc.Id;
                return author;
            }).ToList();
        }

        public async Task<Author?> GetByIdAsync(string id)
        {
            var doc = await _authors.Document(id).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var author = doc.ConvertTo<Author>();
            author.Id = doc.Id;
            return author;
        }

        public async Task<Author?> CreateAsync(AuthorCreateDTO dto)
        {
            var normalizedName = NormalizeName(dto.Name);

            var allAuthors = await GetAllAsync();
            bool isDuplicate = allAuthors.Any(a =>
                string.Equals(a.Name, normalizedName, StringComparison.OrdinalIgnoreCase) &&
                (
                    (a.BirthYear.HasValue && dto.BirthYear.HasValue && a.BirthYear.Value == dto.BirthYear.Value) ||
                    (!a.BirthYear.HasValue && !dto.BirthYear.HasValue)
                ));

            if (isDuplicate)
                return null;

            var author = new Author
            {
                Name = normalizedName,
                Bio = dto.Bio,
                Email = dto.Email,
                BirthYear = dto.BirthYear,
                DeathYear = dto.DeathYear,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            var added = await _authors.AddAsync(author);
            author.Id = added.Id;
            return author;
        }

        public async Task<Author?> UpdateAsync(string id, AuthorUpdateDTO dto)
        {
            var docRef = _authors.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return null;

            var normalizedName = NormalizeName(dto.Name);
            var allAuthors = await GetAllAsync();

            bool isDuplicate = allAuthors.Any(a =>
                a.Id != id &&
                string.Equals(a.Name, normalizedName, StringComparison.OrdinalIgnoreCase) &&
                (
                    (a.BirthYear.HasValue && dto.BirthYear.HasValue && a.BirthYear.Value == dto.BirthYear.Value) ||
                    (!a.BirthYear.HasValue && !dto.BirthYear.HasValue)
                ));

            if (isDuplicate)
                return null;

            var createdAt = snapshot.GetValue<Timestamp?>("created_at") ?? Timestamp.FromDateTime(DateTime.UtcNow);

            var author = new Author
            {
                Name = normalizedName,
                Bio = dto.Bio,
                Email = dto.Email,
                BirthYear = dto.BirthYear,
                DeathYear = dto.DeathYear,
                CreatedAt = createdAt,
                UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            await docRef.SetAsync(author, SetOptions.Overwrite);
            author.Id = id;
            return author;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var booksWithAuthor = await _db.Collection("books")
                .WhereArrayContains("author_ids", id)
                .GetSnapshotAsync();

            if (booksWithAuthor.Count > 0)
                return false;

            var doc = _authors.Document(id);
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
