using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MvcBookApplication.Data.Models
{
    public class GalleryFile
    {
        public int Id { get; set; }
        
        public string Username { get; set; }

        [Required(ErrorMessage = "Original filename is required")]
        public string OriginalFilename { get; set; }

        [Required(ErrorMessage = "Filename is required")]
        public string Filename { get; set; }

    }
}
