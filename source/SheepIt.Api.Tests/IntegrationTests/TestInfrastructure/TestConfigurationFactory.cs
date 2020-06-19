using System.IO;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure.Configuration;

namespace SheepIt.Api.Tests.IntegrationTests.TestInfrastructure
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