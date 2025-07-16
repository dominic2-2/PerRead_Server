using Microsoft.AspNetCore.Mvc;
using PerRead_Server.DTOs;
using PerRead_Server.Models;
using PerRead_Server.Services;

namespace PerRead_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _users;

        public UserController(UserService users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? keyword = null,
            string? role = null,
            string? plan = null,
            bool? isActive = null,
            string? sort = "created_at",
            bool desc = true,
            int page = 1,
            int pageSize = 5)
        {
            var allUsers = await _users.GetUsersAsync();

            var filtered = allUsers
                .Where(u =>
                    (string.IsNullOrEmpty(keyword) ||
                     u.Id.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     (u.FullName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                    (string.IsNullOrEmpty(role) || u.Role == role) &&
                    (string.IsNullOrEmpty(plan) || u.SubscriptionPlan == plan) &&
                    (!isActive.HasValue || u.IsActive == isActive.Value)
                )
                .OrderBy(u => sort == "updated_at" ? u.UpdatedAt : u.CreatedAt)
                .ToList();

            if (desc)
                filtered.Reverse();

            var total = filtered.Count;
            var paged = filtered.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new { total, data = paged });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _users.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO user)
        {
            try
            {
                var created = await _users.CreateAsync(user);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, User user)
        {
            var updated = await _users.UpdateAsync(id, user);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _users.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
