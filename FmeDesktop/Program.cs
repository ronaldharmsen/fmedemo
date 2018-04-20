using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FmeDesktop
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                var path = args[0];
                Console.WriteLine($"Handling file: {path}");
                var content = File.ReadAllText(path);

                var reversed = new string(content.Reverse().ToArray());
                var outputPath = Path.Combine(@"c:\temp\fme\out\", Path.GetFileName(path));

                File.WriteAllText(outputPath, reversed);
                UploadFile(outputPath, "http://localhost:8080/upload"); //post to proxyserver
                File.Delete(path);
                return 0;
            }
            else
                return 1;
        }

        static void UploadFile(string filePath, string url)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            FileStream fs = File.OpenRead(filePath);
            var content = new StreamContent(fs);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            form.Add(content, "file", Path.GetFileName(filePath));
            var response = httpClient.PostAsync(url, form).Result;
        }
    }
}
