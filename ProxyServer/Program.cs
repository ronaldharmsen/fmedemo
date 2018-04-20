using Serilog;
using System;
using Topshelf;
using Topshelf.Nancy;
using Topshelf.Runtime;

namespace ProxyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Serilog.Log.Logger = log;

            var host = HostFactory.Run(x =>
            {
                x.Service<ProxyService>(svc =>
                {
                    svc.ConstructUsing(
                        new ServiceFactory<ProxyService>(
                            settings => new ProxyService(settings, log)));

                    svc.WithNancyEndpoint(x, c =>
                    {
                        c.AddHost(port: 8080);
                        c.CreateUrlReservationsOnInstall();
                        c.ConfigureNancy(config =>
                        {
                            config.UrlReservations.CreateAutomatically = true;
                        });
                    });

                    svc.WhenStarted((s, c) => s.Start(c));
                    svc.WhenStopped((s, c) => s.Stop(c));
                });
                x.RunAsNetworkService();

                x.SetDescription("OS FME Proxy");
                x.SetDisplayName("OS FME Proxy server");
                x.SetServiceName("FMEProxy");

                x.EnableServiceRecovery(recover =>
                {
                    recover.RestartService(1);
                });


                x.UseSerilog();
            });

            var exitCode = (int)Convert.ChangeType(host, host.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
