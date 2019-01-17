using System;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Logger;

namespace SheepIt.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = ConfigurationFactory.CreateConfiguration(args);
            
            Log.Logger = LoggerFactory.CreateLogger(configuration);
            
            try
            {
                Log.Information("Starting web host");

                var containerBuilder = new ContainerBuilder();

                containerBuilder.RegisterModule<SheepItModule>();

                containerBuilder.RegisterInstance(configuration)
                    .As<IConfiguration>()
                    .SingleInstance();

                using (var container = containerBuilder.Build())
                {
                    var webApp = container.Resolve<WebApp>();

                    webApp.StartAndWait(args);
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.ToString(), "Terminated unexpectedly!");
                
                Log.Fatal(exception, "Terminated unexpectedly!");
                
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
