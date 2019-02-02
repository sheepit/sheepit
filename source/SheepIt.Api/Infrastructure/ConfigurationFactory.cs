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

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configurationBuilder.AddJsonFile($"appsettings.{environment}.linux.json", optional: true);
            }

            return configurationBuilder
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}