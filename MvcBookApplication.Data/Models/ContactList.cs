using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcBookApplication.Data.Models
{
    public class ContactList
    {
        public int Id { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"[\w ]+",
            ErrorMessage = "Invalid name")]
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Contact> Contacts { get; set; }
    }
}