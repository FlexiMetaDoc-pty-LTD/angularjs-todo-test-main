// The interface for our to-do management service.
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetTodosAsync(int userId);
        Task<TodoItem> GetTodoAsync(int id, int userId);
        Task<TodoItem> AddTodoAsync(TodoItem todoItem);
        Task<bool> UpdateTodoAsync(TodoItem todoItem);
        Task<bool> DeleteTodoAsync(int id, int userId);
        Task<bool> SaveAllAsync();
    }
}