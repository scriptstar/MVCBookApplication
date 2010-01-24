using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MvcBookApplication.Validation;

namespace MvcBookApplication.Data.Models
{
    public enum Sex
    {
        Undefined = 0,
        Male = 1,
        Female = 2
    }

    public class Contact
    {
        public User User { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [Email(ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public DateTime? Dob { get; set; }
    }
}
