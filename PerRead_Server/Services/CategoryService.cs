using Google.Cloud.Firestore;
using PerRead_Server.Models;
using PerRead_Server.DTOs;
using System.Globalization;

namespace PerRead_Server.Services
{
    public class CategoryService
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _categories;

        public CategoryService(FirestoreService firestore)
        {
            _db = firestore.Db;
            _categories = _db.Collection("categories");
        }

        public async Task<List<Category>> GetAllAsync()
        {
            var snapshot = await _categories.GetSnapshotAsync();
            return snapshot.Documents.Select(doc =>
            {
                var cat = doc.ConvertTo<Category>();
                cat.Id = doc.Id;
                return cat;
            }).ToList();
        }

        public async Task<Category?> GetByIdAsync(string id)
        {
            var doc = await _categories.Document(id).GetSnapshotAsync();
            if (!doc.Exists) return null;

            var category = doc.ConvertTo<Category>();
            category.Id = doc.Id;
            return category;
        }

        public async Task<Category?> CreateAsync(CategoryCreateDTO dto)
        {
            var normalizedName = NormalizeCategoryName(dto.Name);

            var exists = await _categories
                .WhereEqualTo("name", normalizedName)
                .GetSnapshotAsync();

            if (exists.Count > 0)
                return null;

            var category = new Category
            {
                Name = normalizedName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var added = await _categories.AddAsync(category);
            category.Id = added.Id;
            return category;
        }

        public async Task<Category?> UpdateAsync(string id, CategoryUpdateDTO dto)
        {
            var snapshot = await _categories.Document(id).GetSnapshotAsync();
            if (!snapshot.Exists) return null;

            var normalizedName = NormalizeCategoryName(dto.Name);

            var duplicates = await _categories
                .WhereEqualTo("name", normalizedName)
                .GetSnapshotAsync();

            var isDuplicate = duplicates.Documents.Any(doc => doc.Id != id);
            if (isDuplicate)
                return null;

            var updated = new Category
            {
                Name = normalizedName,
                CreatedAt = snapshot.GetValue<DateTime>("created_at"),
                UpdatedAt = DateTime.UtcNow
            };

            await _categories.Document(id).SetAsync(updated, SetOptions.Overwrite);
            updated.Id = id;
            return updated;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var booksCollection = _db.Collection("books");
            var query = booksCollection.WhereArrayContains("category_ids", id);
            var booksWithCategory = await query.GetSnapshotAsync();

            if (booksWithCategory.Count > 0)
                return false;

            var doc = _categories.Document(id);
            var snapshot = await doc.GetSnapshotAsync();
            if (!snapshot.Exists) return false;

            await doc.DeleteAsync();
            return true;
        }

        private string NormalizeCategoryName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            var culture = CultureInfo.CurrentCulture;
            return culture.TextInfo.ToTitleCase(name.Trim().ToLower());
        }
    }
}
