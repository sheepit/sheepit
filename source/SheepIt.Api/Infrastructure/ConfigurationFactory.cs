using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace SheepIt.Api.Infrastructure
{
    internal class ConfigurationFactory
    {
        private static readonly ILogger Logger = Log.ForContext<ConfigurationFactory>();
        
        public static IConfiguration CreateConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower();
            
            Logger.Information("Environment variable: '{environment}'", environment);
            
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"appsettings.{environment}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }
    }

}