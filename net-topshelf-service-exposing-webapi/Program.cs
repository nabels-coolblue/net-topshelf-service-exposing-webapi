using Microsoft.Owin.Hosting;
using Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Topshelf;
using ServiceHealthReporting.Core;

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

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }
    }

    public class HealthController : ServiceHealthReporting.Web.HealthController
    {
        public HealthController(IHealthChecksRunner healthChecksRunner) : base(healthChecksRunner)
        {
        }
    }

    public class DemoController : ApiController
    {
        // GET api/demo 
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "World" };
        }

        // GET api/demo/5 
        public string Get(int id)
        {
            return "Hello, World!";
        }

        // POST api/demo 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/demo/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/demo/5 
        public void Delete(int id)
        {
        }
    }

    public class WindowsHttpApiService
    {
        IDisposable _WebApp;

        public void Start()
        {
            _WebApp = WebApp.Start<Startup>("http://localhost:8080");

            Console.WriteLine("Web Server is running.");
            Console.WriteLine("Press any key to quit.");
            Console.ReadLine();
            
            Log.Logger.Information("Service started");
        }

        public void Stop()
        {
            _WebApp?.Dispose();

            Log.Logger.Information("Service stopped");
        }
    }
}
