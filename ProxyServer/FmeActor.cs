using Akka.Actor;

namespace ProxyServer
{
    public class FmeActor : ReceiveActor
    {
        private readonly Serilog.ILogger log;

        public FmeActor(Serilog.ILogger log)
        {
            this.log = log;
            Receive<DoThis>(msg => {
                log.Information($"Incoming request for {msg.File}");
                var download = Context.ActorOf(Props.Create(() => new DownloadFileActor(log)));
                download.Tell(msg.File);
            });

            Receive<FileDownloaded>(msg =>
            {
                var fmeDesktop = Context.ActorOf(Props.Create(() => new FmeDesktopActor(log)));
                fmeDesktop.Tell(new ProcessFile(msg.File));
            });

            Receive<FileProcessed>(msg =>
            {
                log.Information($"Uploading file: {msg.File}");
                log.Information($"File completed: {msg.File}");
            });
        }
    }
}