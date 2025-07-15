using Microsoft.AspNetCore.Mvc;
using PerRead_Server.DTOs;
using PerRead_Server.Models;
using PerRead_Server.Services;

namespace PerRead_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookService _service;

        public BookController(BookService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? keyword = null,
            [FromQuery] List<string>? authorIds = null,
            string? publisherId = null,
            [FromQuery] List<string>? categoryIds = null,
            bool? availability = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            [FromQuery] List<string>? sort = null,
            int page = 1,
            int pageSize = 5)
        {
            var allBooks = await _service.GetAllAsync();

            var query = allBooks
                .Where(b =>
                    (string.IsNullOrEmpty(keyword) ||
                        b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        b.Summary.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        b.ISBN.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        b.Language.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        (b.Authors != null && b.Authors.Any(a => a.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))) ||
                        (b.Categories != null && b.Categories.Any(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))) ||
                        (b.Publisher != null && b.Publisher.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    ) &&
                    (authorIds == null || authorIds.Count == 0 || (b.Authors != null && b.Authors.Any(a => authorIds.Contains(a.Id)))) &&
                    (string.IsNullOrEmpty(publisherId) || (b.Publisher != null && b.Publisher.Id == publisherId)) &&
                    (categoryIds == null || categoryIds.Count == 0 || (b.Categories != null && b.Categories.Any(c => categoryIds.Contains(c.Id)))) &&
                    (!availability.HasValue || b.Availability == availability.Value) &&
                    (!priceMin.HasValue || b.Price >= priceMin.Value) &&
                    (!priceMax.HasValue || b.Price <= priceMax.Value)
                )
                .AsQueryable();

            IOrderedQueryable<BookDetailDTO>? orderedQuery = null;

            if (sort != null && sort.Count > 0)
            {
                foreach (var sortExpr in sort)
                {
                    var parts = sortExpr.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    var field = parts[0].Trim().ToLower();
                    var direction = (parts.Length > 1 ? parts[1] : "asc").Trim().ToLower();

                    bool desc = direction == "desc";

                    if (orderedQuery == null)
                    {
                        orderedQuery = field switch
                        {
                            "title" => desc ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                            "price" => desc ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
                            "updated_at" => desc ? query.OrderByDescending(b => b.UpdatedAt) : query.OrderBy(b => b.UpdatedAt),
                            "created_at" => desc ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt),
                            _ => query.OrderBy(b => b.Title)
                        };
                    }
                    else
                    {
                        orderedQuery = field switch
                        {
                            "title" => desc ? orderedQuery.ThenByDescending(b => b.Title) : orderedQuery.ThenBy(b => b.Title),
                            "price" => desc ? orderedQuery.ThenByDescending(b => b.Price) : orderedQuery.ThenBy(b => b.Price),
                            "updated_at" => desc ? orderedQuery.ThenByDescending(b => b.UpdatedAt) : orderedQuery.ThenBy(b => b.UpdatedAt),
                            "created_at" => desc ? orderedQuery.ThenByDescending(b => b.CreatedAt) : orderedQuery.ThenBy(b => b.CreatedAt),
                            _ => orderedQuery
                        };
                    }
                }
            }
            else
            {
                orderedQuery = query.OrderByDescending(b => b.CreatedAt);
            }

            var total = orderedQuery?.Count() ?? 0;
            var paged = (orderedQuery ?? Enumerable.Empty<BookDetailDTO>().AsQueryable())
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new { total, data = paged });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var book = await _service.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Title is required." });

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BookUpdateDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated is null
                ? NotFound(new { message = "Book not found." })
                : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
