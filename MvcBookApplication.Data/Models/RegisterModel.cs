using System.ComponentModel.DataAnnotations;

namespace MvcBookApplication.Data.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage="Username is required")]
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}