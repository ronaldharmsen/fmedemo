using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Infrastructure
{
    public class FileStorage
    {
        CloudStorageAccount storageAccount;
        private CloudBlobClient blob;
        private CloudQueueClient queueClient;
        private CloudQueue q;
        private CloudBlobContainer container;

        public FileStorage(IConfiguration config)
        {
            storageAccount = CloudStorageAccount.Parse(config["StorageConnectionString"]);
            blob = storageAccount.CreateCloudBlobClient();
            container = blob.GetContainerReference("files");
            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            queueClient = storageAccount.CreateCloudQueueClient();
            q = queueClient.GetQueueReference("processwithfme");
        }

        public async Task UploadFile(string name, string content)
        {            
            var file = container.GetBlockBlobReference(name);
            await file.DeleteIfExistsAsync();

            await file.UploadTextAsync(content);

            await q.AddMessageAsync(new CloudQueueMessage(name));   
        }
    }
}
