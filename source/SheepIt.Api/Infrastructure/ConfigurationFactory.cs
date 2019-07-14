using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Infrastructure
{
    internal class ConfigurationFactory
    {
        public static IConfiguration CreateConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower();

            Console.WriteLine($"Preparing configuration for {environment}");
            
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            return configurationBuilder
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}