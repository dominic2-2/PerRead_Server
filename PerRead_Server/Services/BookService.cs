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

        public async Task<List<BookDetailDTO>> GetAllAsync()
        {
            var bookSnapshot = await _books.GetSnapshotAsync();
            var books = bookSnapshot.Documents.Select(doc =>
            {
                var book = doc.ConvertTo<Book>();
                book.Id = doc.Id;
                return book;
            }).ToList();

            // Lấy dữ liệu liên quan
            var authorSnapshot = await _db.Collection("authors").GetSnapshotAsync();
            var authors = authorSnapshot.Documents.Select(d =>
            {
                var a = d.ConvertTo<Author>();
                a.Id = d.Id;
                return a;
            }).ToList();

            var categorySnapshot = await _db.Collection("categories").GetSnapshotAsync();
            var categories = categorySnapshot.Documents.Select(d =>
            {
                var c = d.ConvertTo<Category>();
                c.Id = d.Id;
                return c;
            }).ToList();

            var publisherSnapshot = await _db.Collection("publishers").GetSnapshotAsync();
            var publishers = publisherSnapshot.Documents.Select(d =>
            {
                var p = d.ConvertTo<Publisher>();
                p.Id = d.Id;
                return p;
            }).ToList();

            // Join dữ liệu
            var result = books.Select(b => new BookDetailDTO
            {
                Id = b.Id,
                Title = b.Title,
                Authors = authors.Where(a => b.AuthorIds.Contains(a.Id)).ToList(),
                Categories = categories.Where(c => b.CategoryIds.Contains(c.Id)).ToList(),
                Publisher = publishers.FirstOrDefault(p => p.Id == b.PublisherId),
                Language = b.Language,
                Edition = b.Edition,
                ISBN = b.ISBN,
                Pages = b.Pages,
                Summary = b.Summary,
                CoverUrl = b.CoverUrl,
                FileUrl = b.FileUrl,
                FileSize = b.FileSize,
                Price = b.Price,
                Availability = b.Availability,
                Tags = b.Tags,
                AverageRating = b.AverageRating,
                TotalReviews = b.TotalReviews,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();

            return result;
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
            // Check PaymentTransaction
            var paymentQuery = await _db.Collection("paymentTransactions")
                .WhereEqualTo("book_id", id)
                .Limit(1)
                .GetSnapshotAsync();
            if (paymentQuery.Count > 0) return false;

            // Check ReadingProgress
            var readingProgressQuery = await _db.Collection("readingProgress")
                .WhereEqualTo("book_id", id)
                .Limit(1)
                .GetSnapshotAsync();
            if (readingProgressQuery.Count > 0) return false;

            // Check UserFavourite
            var userFavouriteQuery = await _db.Collection("userFavourites")
                .WhereArrayContains("book_ids", id)
                .Limit(1)
                .GetSnapshotAsync();
            if (userFavouriteQuery.Count > 0) return false;

            // Check UserPurchasedBook
            var userPurchasedBookQuery = await _db.Collection("userPurchasedBooks")
                .WhereArrayContains("book_ids", id)
                .Limit(1)
                .GetSnapshotAsync();
            if (userPurchasedBookQuery.Count > 0) return false;

            // Check BookCollection
            var bookCollectionQuery = await _db.Collection("bookCollections")
                .WhereArrayContains("book_ids", id)
                .Limit(1)
                .GetSnapshotAsync();
            if (bookCollectionQuery.Count > 0) return false;

            var doc = _books.Document(id);
            var snapshot = await doc.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await doc.DeleteAsync();
            return true;
        }
    }
}
