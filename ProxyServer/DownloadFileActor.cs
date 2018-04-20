using Akka.Actor;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using System.IO;

namespace ProxyServer
{
    public class DownloadFileActor: ReceiveActor
    {
        private ILogger log;

        public DownloadFileActor(ILogger log)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("files");
            container.CreateIfNotExists();

            this.log = log;
            ReceiveAsync<string>(async file =>
            {
                log.Information($"File downloaded: {file}");
                var blob = container.GetBlobReference(file);
                if (await blob.ExistsAsync())
                {
                    var localFile = Path.Combine(@"c:\temp\fme\in\", file);
                    await blob.DownloadToFileAsync(localFile, FileMode.Create);
                    Sender.Tell(new FileDownloaded() { File = localFile });
                }                
            });
        }
    }
}