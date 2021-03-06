﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Configuration;
using SheepIt.Api.Infrastructure.Logger;

[assembly: InternalsVisibleTo("SheepIt.Api.Tests")]

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
                Log.Fatal(exception, "Terminated unexpectedly!");
                
                Console.WriteLine("Terminated unexpectedly!");
                Console.Error.WriteLine(exception.ToString());
                
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
