using Microsoft.AspNetCore.Mvc;
using PerRead_Server.DTOs;
using PerRead_Server.DTOs.PerRead_Server.DTOs;
using PerRead_Server.Models;
using PerRead_Server.Services;

namespace PerRead_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublisherController : ControllerBase
    {
        private readonly PublisherService _service;

        public PublisherController(PublisherService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? keyword = null,
            string? website = null,
            string? sort = "name",
            bool desc = false,
            int page = 1,
            int pageSize = 5)
        {
            var all = await _service.GetAllAsync();

            var filtered = all.Where(p =>
                (string.IsNullOrEmpty(keyword) ||
                    p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (p.Email?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Address?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                (string.IsNullOrEmpty(website) || (p.Website?.Contains(website, StringComparison.OrdinalIgnoreCase) ?? false))
            ).ToList();

            filtered = sort switch
            {
                "name" => desc ? filtered.OrderByDescending(p => p.Name).ToList() : filtered.OrderBy(p => p.Name).ToList(),
                _ => filtered
            };

            var total = filtered.Count;
            var paged = filtered.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new { total, data = paged });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var pub = await _service.GetByIdAsync(id);
            return pub is null ? NotFound() : Ok(pub);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PublisherCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Publisher name is required." });

            var created = await _service.CreateAsync(dto);
            return created is null
                ? Conflict(new { message = "Publisher with the same name already exists." })
                : Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PublisherUpdateDTO dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated is null
                ? Conflict(new { message = "Duplicate name or invalid publisher ID." })
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
