using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using System.Security.Claims;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue("id"));

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var total = await _context.Todos.CountAsync(t => t.UserId == userId);
            var completed = await _context.Todos.CountAsync(t => t.UserId == userId && t.Completed);
            var pending = total - completed;

            return Ok(new
            {
                total,
                completed,
                pending
            });
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetCompletedByDate()
        {
            var userId = GetUserId();
            var grouped = await _context.Todos
                .Where(t => t.UserId == userId && t.CompletedAt != null)
                .GroupBy(t => t.CompletedAt.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(grouped);
        }

    }
}