using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace net_topshelf_service_exposing_webapi
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(configurator =>
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.AppSettings()
                    .WriteTo.ColoredConsole()
                    .CreateLogger();
                Log.Logger.Information("Logger configured");

                configurator.UseSerilog();
                configurator.SetServiceName("Windows.Service.ApiController");
                configurator.Service<WindowsHttpApiService>(s =>
                {
                    s.ConstructUsing(() => new WindowsHttpApiService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
            });
        }
    }

    public class WindowsHttpApiService
    {
        public void Start()
        {
            Log.Logger.Information("Service started");
        }

        public void Stop()
        {
            Log.Logger.Information("Service stopped");
        }
    }
}
