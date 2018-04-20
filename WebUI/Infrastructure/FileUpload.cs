using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Infrastructure
{
    public class FileUpload
    {
        [Required]
        [Display(Name = "Text file")]
        public IFormFile TextFile { get; set; }
    }
}
