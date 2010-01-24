using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MvcBookApplication.Data.Models
{
    public class Message
    {
        public User User { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }
        
        public string Html { get; set; }
        
        [Required(ErrorMessage = "A plain text body is required")]
        public string Text { get; set; }
        
        public int Id { get; set; }
    }
}
