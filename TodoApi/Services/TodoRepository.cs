// The concrete implementation for the to-do service.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoRepository : ITodoRepository
    {
        private readonly DataContext _context;

        public TodoRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetTodosAsync(int userId)
        {
            // Retrieve all to-do items for a specific user.
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<TodoItem> GetTodoAsync(int id, int userId)
        {
            // Retrieve a single to-do item, ensuring it belongs to the user.
            return await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<TodoItem> AddTodoAsync(TodoItem todoItem)
        {
            await _context.TodoItems.AddAsync(todoItem);
            return todoItem;
        }

        public async Task<bool> UpdateTodoAsync(TodoItem todoItem)
        {
            _context.TodoItems.Update(todoItem);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteTodoAsync(int id, int userId)
        {
            var todoToDelete = await GetTodoAsync(id, userId);
            if (todoToDelete == null)
                return false; // Item not found or doesn't belong to user

            _context.TodoItems.Remove(todoToDelete);
            return await SaveAllAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}