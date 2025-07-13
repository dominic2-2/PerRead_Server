using Microsoft.AspNetCore.Mvc;
using PerRead_Server.Models;
using PerRead_Server.Services;
using PerRead_Server.DTOs;

namespace PerRead_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? keyword = null,
            string? sort = "created_at",
            bool desc = true,
            int page = 1,
            int pageSize = 5)
        {
            var all = await _service.GetAllAsync();

            var filtered = all.Where(c =>
                string.IsNullOrEmpty(keyword) ||
                c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            filtered = sort switch
            {
                "updated_at" => desc ? filtered.OrderByDescending(c => c.UpdatedAt).ToList() : filtered.OrderBy(c => c.UpdatedAt).ToList(),
                _ => desc ? filtered.OrderByDescending(c => c.CreatedAt).ToList() : filtered.OrderBy(c => c.CreatedAt).ToList()
            };

            var total = filtered.Count;
            var paged = filtered.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new { total, data = paged });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var cat = await _service.GetByIdAsync(id);
            return cat is null ? NotFound() : Ok(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Category name is required." });

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CategoryUpdateDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
