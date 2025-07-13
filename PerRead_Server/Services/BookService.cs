using Google.Cloud.Firestore;
using PerRead_Server.Models;
using PerRead_Server.DTOs;

namespace PerRead_Server.Services
{
    public class BookService
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _books;

        public BookService(FirestoreService firestore)
        {
            _db = firestore.Db;
            _books = _db.Collection("books");
        }

        public async Task<List<Book>> GetAllAsync()
        {
            var snapshot = await _books.GetSnapshotAsync();
            return snapshot.Documents.Select(doc =>
            {
                var book = doc.ConvertTo<Book>();
                book.Id = doc.Id;
                return book;
            }).ToList();
        }

        public async Task<Book?> GetByIdAsync(string id)
        {
            var doc = await _books.Document(id).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var book = doc.ConvertTo<Book>();
            book.Id = doc.Id;
            return book;
        }

        public async Task<Book> CreateAsync(BookCreateDTO dto)
        {
            var now = DateTime.UtcNow;
            var book = new Book
            {
                Title = dto.Title,
                AuthorIds = dto.AuthorIds,
                PublisherId = dto.PublisherId,
                CategoryIds = dto.CategoryIds,
                Language = dto.Language,
                Edition = dto.Edition,
                ISBN = dto.ISBN,
                Pages = dto.Pages,
                Summary = dto.Summary,
                CoverUrl = dto.CoverUrl,
                FileUrl = dto.FileUrl,
                FileSize = dto.FileSize,
                Price = dto.Price,
                Availability = dto.Availability,
                Tags = dto.Tags,
                AverageRating = 0,
                TotalReviews = 0,
                CreatedAt = now,
                UpdatedAt = now
            };

            var added = await _books.AddAsync(book);
            book.Id = added.Id;
            return book;
        }

        public async Task<Book?> UpdateAsync(string id, BookUpdateDTO dto)
        {
            var docRef = _books.Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return null;

            var createdAt = snapshot.GetValue<DateTime?>("created_at") ?? DateTime.UtcNow;

            var book = new Book
            {
                Title = dto.Title,
                AuthorIds = dto.AuthorIds,
                PublisherId = dto.PublisherId,
                CategoryIds = dto.CategoryIds,
                Language = dto.Language,
                Edition = dto.Edition,
                ISBN = dto.ISBN,
                Pages = dto.Pages,
                Summary = dto.Summary,
                CoverUrl = dto.CoverUrl,
                FileUrl = dto.FileUrl,
                FileSize = dto.FileSize,
                Price = dto.Price,
                Availability = dto.Availability,
                Tags = dto.Tags,
                AverageRating = snapshot.GetValue<double?>("average_rating") ?? 0,
                TotalReviews = snapshot.GetValue<int?>("total_reviews") ?? 0,
                CreatedAt = createdAt,
                UpdatedAt = DateTime.UtcNow
            };

            await docRef.SetAsync(book, SetOptions.Overwrite);
            book.Id = id;
            return book;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var doc = _books.Document(id);
            var snapshot = await doc.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await doc.DeleteAsync();
            return true;
        }
    }
}
