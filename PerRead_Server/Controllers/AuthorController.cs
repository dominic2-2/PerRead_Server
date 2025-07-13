using Microsoft.AspNetCore.Mvc;
using PerRead_Server.DTOs;
using PerRead_Server.Models;
using PerRead_Server.Services;

namespace PerRead_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly AuthorService _service;

        public AuthorController(AuthorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? keyword = null,
            int? birthYear = null,
            int? deathYear = null,
            string? sort = "created_at",
            bool desc = true,
            int page = 1,
            int pageSize = 5)
        {
            var all = await _service.GetAllAsync();

            var filtered = all.Where(a =>
                (string.IsNullOrEmpty(keyword) ||
                    (a.Name?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Email?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (!birthYear.HasValue || a.BirthYear == birthYear) &&
                (!deathYear.HasValue || a.DeathYear == deathYear)
            ).ToList();

            filtered = sort switch
            {
                "name" => desc ? filtered.OrderByDescending(a => a.Name).ToList() : filtered.OrderBy(a => a.Name).ToList(),
                "updated_at" => desc ? filtered.OrderByDescending(a => a.UpdatedAt).ToList() : filtered.OrderBy(a => a.UpdatedAt).ToList(),
                _ => desc ? filtered.OrderByDescending(a => a.CreatedAt).ToList() : filtered.OrderBy(a => a.CreatedAt).ToList()
            };

            var total = filtered.Count;
            var paged = filtered.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new { total, data = paged });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var author = await _service.GetByIdAsync(id);
            return author is null ? NotFound() : Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Author name is required." });

            var created = await _service.CreateAsync(dto);
            return created is null
                ? Conflict(new { message = "Author with same name and birth year already exists." })
                : Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AuthorUpdateDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated is null
                ? Conflict(new { message = "Duplicate author found or invalid ID." })
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
