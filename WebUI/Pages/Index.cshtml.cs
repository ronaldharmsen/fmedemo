using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using WebUI.Infrastructure;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public FileUpload FileUpload { get; set; }
        private FileStorage storage;
        public IndexModel(IConfiguration configuration)
        {
            storage = new FileStorage(configuration);
        }
        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fileName = WebUtility.HtmlEncode(Path.GetFileName(FileUpload.TextFile.FileName));
            var textFile =
                await FileHelpers.ProcessFormFile(FileUpload.TextFile, ModelState);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await storage.UploadFile(fileName, textFile);
            return RedirectToPage("./index");
        }

    }
}
