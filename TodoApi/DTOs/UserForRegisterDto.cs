using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs
{
    // Data Transfer Object for user registration.
    public class UserForRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 20 characters.")]
        public string Password { get; set; }
    }
}
