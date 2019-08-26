using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Logger;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Web;

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

                    var idProvider = container.Resolve<IdentityProvider>();
                    idProvider
                        .InitializeIdentities(new List<string>
                        {
                            "Environment"
                        })
                        .GetAwaiter()
                        .GetResult();
                    
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
