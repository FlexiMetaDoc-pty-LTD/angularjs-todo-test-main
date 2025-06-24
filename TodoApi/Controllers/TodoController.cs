// The API Controller for all To-Do operations. Secured with [Authorize].
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _repo;
        //private readonly Hub _hubContext;

        public TodoController(ITodoRepository repo)
        {
            _repo = repo;
            // _hubContext = hubContext;
        }

        // GET api/todo
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            // Get the user ID from the token's claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var todos = await _repo.GetTodosAsync(userId);

            // Map to DTOs to control the output
            var todosToReturn = new List<TodoItemDto>();
            foreach (var todo in todos)
            {
                todosToReturn.Add(new TodoItemDto { Id = todo.Id, Task = todo.Task, IsDone = todo.IsDone, CreatedDate = todo.CreatedDate });
            }

            return Ok(todosToReturn);
        }

        // POST api/todo
        //[HttpPost]
        //public async Task<IActionResult> CreateTodo(TodoItemDto todoForCreationDto)
        //{
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        //    var todo = new TodoItem
        //    {
        //        Task = todoForCreationDto.Task,
        //        IsDone = false,
        //        CreatedDate = DateTime.Now,
        //        UserId = userId
        //    };

        //    var createdTodo = await _repo.AddTodoAsync(todo);
        //    await _repo.SaveAllAsync();

        //    var todoToReturn = new TodoItemDto
        //    {
        //        Id = createdTodo.Id,
        //        Task = createdTodo.Task,
        //        IsDone = createdTodo.IsDone,
        //        CreatedDate = createdTodo.CreatedDate
        //    };

        //    //Broadcast new todo
        //    await _hubContext.Clients.All.SendAsync("TodoAdded", todoToReturn);

        //    return Ok(todoToReturn);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateTodo(TodoItemDto todoForCreationDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var todo = new TodoItem
            {
                Task = todoForCreationDto.Task,
                IsDone = false,
                CreatedDate = DateTime.Now,
                UserId = userId
            };

            var createdTodo = await _repo.AddTodoAsync(todo);
            await _repo.SaveAllAsync();

            var todoToReturn = new TodoItemDto { Id = createdTodo.Id, Task = createdTodo.Task, IsDone = createdTodo.IsDone, CreatedDate = createdTodo.CreatedDate };

            return Ok(todoToReturn);
        }

        // PUT api/todo/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTodo(int id, TodoItemDto todoForUpdateDto)
        //{
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var todoFromRepo = await _repo.GetTodoAsync(id, userId);

        //    if (todoFromRepo == null)
        //        return Unauthorized("You are not authorized to update this item or it does not exist.");

        //    todoFromRepo.Task = todoForUpdateDto.Task;
        //    todoFromRepo.IsDone = todoForUpdateDto.IsDone;

        //    // Create DTO to broadcast
        //    var todoDto = new TodoItemDto
        //    {
        //        Id = todoFromRepo.Id,
        //        Task = todoFromRepo.Task,
        //        IsDone = todoFromRepo.IsDone,
        //        CreatedDate = todoFromRepo.CreatedDate
        //    };

        //    await _hubContext.Clients.All.SendAsync("TodoUpdated", todoDto);

        //    if (await _repo.UpdateTodoAsync(todoFromRepo))
        //        return NoContent();

        //    throw new Exception($"Updating to-do item {id} failed on save.");
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, TodoItemDto todoForUpdateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var todoFromRepo = await _repo.GetTodoAsync(id, userId);

            if (todoFromRepo == null)
                return Unauthorized("You are not authorized to update this item or it does not exist.");

            todoFromRepo.Task = todoForUpdateDto.Task;
            todoFromRepo.IsDone = todoForUpdateDto.IsDone;
            //await _hubContext.Clients.All.SendAsync("TodoUpdated", updatedTodo);

            if (await _repo.UpdateTodoAsync(todoFromRepo))
                return NoContent(); // Success, no content to return

            throw new Exception($"Updating to-do item {id} failed on save.");
        }

        //// DELETE api/todo/{id}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTodo(int id)
        //{
        //    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        //    if (await _repo.DeleteTodoAsync(id, userId))
        //    {
        //        //Broadcast delete
        //        await _hubContext.Clients.All.SendAsync("TodoDeleted", id);
        //        return NoContent();
        //    }

        //    return Unauthorized("You are not authorized to delete this item or it does not exist.");
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // The DeleteTodoAsync method already contains the logic to check ownership
            if (await _repo.DeleteTodoAsync(id, userId))
                return NoContent(); // Success

            return Unauthorized("You are not authorized to delete this item or it does not exist.");
        }
    }
}
