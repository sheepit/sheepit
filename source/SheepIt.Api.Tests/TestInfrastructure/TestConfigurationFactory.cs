using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure.Configuration;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    internal static class TestConfigurationFactory
    {
        public static IConfiguration Build()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddPrimaryJsonFile()
                .AddOperatingSystemJsonFile()
                .WriteConfigurationSourcesToConsole()
                .Build();
        }
    }
}