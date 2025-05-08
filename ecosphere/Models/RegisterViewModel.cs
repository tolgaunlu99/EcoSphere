using System.ComponentModel.DataAnnotations;

namespace EcoSphere.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
