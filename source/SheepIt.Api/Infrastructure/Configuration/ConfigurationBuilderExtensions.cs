using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.Infrastructure.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddPrimaryJsonFile(this IConfigurationBuilder configuration)
        {
            return configuration.AddJsonFile(
                path: "appsettings.json",
                optional: false
            );
        }

        public static IConfigurationBuilder AddOperatingSystemJsonFile(this IConfigurationBuilder configuration)
        {
            return configuration.AddJsonFile(
                path: $"appsettings.os-{GetOperatingSystemName()}.json",
                optional: true
            );
        }

        private static string GetOperatingSystemName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "windows";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "osx";
            }

            return "unknown-os";
        }
        
        public static IConfigurationBuilder AddEnvironmentJsonFile(this IConfigurationBuilder configuration)
        {
            return configuration.AddJsonFile(
                path: $"appsettings.env-{GetEnvironmentName()}.json",
                optional: true
            );
        }

        private static string GetEnvironmentName()
        {
            return Environment
                .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?.ToLower();
        }

        public static IConfigurationBuilder WriteConfigurationSourcesToConsole(this IConfigurationBuilder configuration)
        {
            var sources = configuration.Sources
                .Select(SourceDisplayName)
                .JoinWith(", ");

            Console.WriteLine($"Preparing configuration from sources: {sources}.");
            
            return configuration;
        }

        private static string SourceDisplayName(IConfigurationSource source)
        {
            if (source is JsonConfigurationSource jsonSource)
            {
                return $"json file: {jsonSource.Path}";
            }

            if (source is CommandLineConfigurationSource)
            {
                return "command line parameters";
            }

            if (source is EnvironmentVariablesConfigurationSource)
            {
                return "environment variables";
            }

            return source.ToString();
        }
    }
}