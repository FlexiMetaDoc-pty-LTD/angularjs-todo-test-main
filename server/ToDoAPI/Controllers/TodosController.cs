using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoAPI.Data;
using ToDoAPI.DTO;
using ToDoAPI.Models;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodosController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue("id"));

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var todos = await _context.Todos
                .Where(t => t.UserId == GetUserId())
                .ToListAsync();

            return Ok(todos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(TodoDTO dto)
        {
            var todo = new Todo
            {
                Text = dto.Text,
                Completed = dto.Completed,
                UserId = GetUserId()
            };

            await _context.Todos.AddAsync(todo);
            await _context.SaveChangesAsync();

            return Ok(todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, TodoDTO dto)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null || todo.UserId != GetUserId())
                return NotFound();

            todo.Text = dto.Text;
            todo.Completed = dto.Completed;
            todo.CompletedAt = dto.Completed ? DateTime.Now : null;

            await _context.SaveChangesAsync();
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null || todo.UserId != GetUserId())
                return NotFound();

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}