using Akka.Actor;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Serilog;
using System;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Runtime;

namespace ProxyServer
{
    class ProxyService : ServiceControl
    {
        private readonly HostSettings settings;
        private readonly ILogger log;
        private ActorSystem system;
        private IActorRef fmeActor;
        private object queuePoller;

        public ProxyService(HostSettings settings, ILogger logger)
        {
            this.settings = settings;
            this.log = logger;
        }

        public bool Start(HostControl hostControl)
        {
            system = ActorSystem.Create("proxy");
            fmeActor = system.ActorOf(Props.Create(() => new FmeActor(this.log)), "fme");

            StartQueuePoller(fmeActor);

            return true;
        }

        private void StartQueuePoller(IActorRef actor)
        {
            int currentBackoff = 0;
            int maximumBackoff = 10;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var client = storageAccount.CreateCloudQueueClient();
            var q = client.GetQueueReference("processwithfme");
            q.CreateIfNotExists();


            queuePoller = Task.Run(async () =>
            {
                while (true)
                {
                    var message = await q.GetMessageAsync();
                    if (message != null)
                    {
                        // Reset backoff
                        currentBackoff = 0;
                        var fileName = message.AsString;
                        fmeActor.Tell(new DoThis(fileName));
                        await q.DeleteMessageAsync(message);
                    }
                    else
                    {
                        if (currentBackoff < maximumBackoff)
                        {
                            currentBackoff++;
                        }
                        await Task.Delay(currentBackoff * 1000);
                    }
                }
            });
        }

        public bool Stop(HostControl hostControl)
        {
            system.Dispose();
            return true;
        }
    }
}
