using System;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Logger;

namespace SheepIt.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = ConfigurationFactory.CreateConfiguration();
            Log.Logger = LoggerFactory.CreateLogger(configuration);
            
            try
            {
                Log.Information("Starting web host");

                var containerBuilder = new ContainerBuilder();

                containerBuilder.RegisterModule<SheepItModule>();

                using (var container = containerBuilder.Build())
                {
                    var webApp = container.Resolve<WebApp>();

                    webApp.Run(args);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString(), "Terminated unexpectedly!");
                Log.Fatal(e, "Terminated unexpectedly!");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
        }
    }
}
