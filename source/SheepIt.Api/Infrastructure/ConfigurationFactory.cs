using System.IO;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure.Configuration;

namespace SheepIt.Api.Infrastructure
{
    internal static class ConfigurationFactory
    {
        public static IConfiguration CreateConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddPrimaryJsonFile()
                .AddOperatingSystemJsonFile()
                .AddEnvironmentJsonFile()
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .WriteConfigurationSourcesToConsole()
                .Build();
        }
    }
}