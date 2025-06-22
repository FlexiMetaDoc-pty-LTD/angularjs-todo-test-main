using System;

namespace TodoApi.Models
{
    // Represents a single To-Do item linked to a user.
    public class TodoItem
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedDate { get; set; }

        // Foreign Key for User
        public int UserId { get; set; }
        public User User { get; set; }
    }
}