using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;

        public int UserId { get; set; }

        public User User { get; set; }
    }
}