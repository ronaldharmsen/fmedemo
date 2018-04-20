using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class FileDownloadProxyModule : NancyModule
    {
        public FileDownloadProxyModule()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var client  = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("fmeresponses");
            container.CreateIfNotExists();

            Get["/"] = _ => "Hello! Please post to /upload";
            Post["/upload", runAsync: true] = async (req, token) =>
             {
                 var postedFile = Request.Files.FirstOrDefault();
                 if (postedFile == null)
                 {
                     return Negotiate
                        .WithModel("No file uploaded.")
                        .WithStatusCode(HttpStatusCode.BadRequest);
                 }

                 try
                 {
                     var correctedFileName = postedFile.Name.ToLower();
                     if (correctedFileName.Contains(";"))
                     {
                         correctedFileName = 
                            correctedFileName.Substring(0, correctedFileName.IndexOf(';'));
                     }

                     var blob = container.GetBlockBlobReference(correctedFileName);
                     await blob.DeleteIfExistsAsync();                     
                     await blob.UploadFromStreamAsync(postedFile.Value);
                 }
                 catch(Exception ex)
                 {
                     return Negotiate
                        .WithModel("Error while uploading...")
                        .WithStatusCode(HttpStatusCode.InternalServerError);
                 }
                 return "Upload completed"; //Automatic 200 OK
             };
            
        }
    }
}
