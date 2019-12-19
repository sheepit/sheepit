using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Configuration;
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
                var hostBuilder = Host
                    .CreateDefaultBuilder(args)
                    .UseSerilog()
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                        webBuilder.UseStartup<Startup>();
                    });
            
                hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddOperatingSystemJsonFile();
                    config.AddEnvironmentJsonFile();
                });

                var host = hostBuilder.Build();

                Log.Information("Starting web host");

                host.Run();
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
        
//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder.UseStartup<Startup>();
//                });
    }
}
