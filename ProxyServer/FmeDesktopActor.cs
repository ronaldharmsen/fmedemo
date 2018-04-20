using Akka.Actor;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ProxyServer
{
    public class FmeDesktopActor : ReceiveActor
    {
        private ILogger log;
        private Process fme;
        private string fmePath;
        public FmeDesktopActor(ILogger log)
        {
            this.log = log;
            fmePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"..\..\..\FmeDesktop\bin\Debug\fmedesktop.exe"
                );

            Become(Ready);
        }

        private void Ready()
        {
            Receive<ProcessFile>(f =>
            {
                log.Information("Handing off request to FmeDesktop");

                var start = new ProcessStartInfo(
                    fmePath,
                    f.File);
                
                fme = Process.Start(start);

                fme.EnableRaisingEvents = true;
                fme.Exited += (s, e) =>
                {
                    log.Information($"Completed with exit code {fme.ExitCode}");
                    Sender.Tell(fme.ExitCode == 0);
                };
                fme.WaitForExit();
                if (fme.ExitCode == 0)
                {
                    Sender.Tell(new FileProcessed(Path.Combine(@"c:\temp\fme\out\", Path.GetFileName(f.File))));
                }
            });
        }
    }
}