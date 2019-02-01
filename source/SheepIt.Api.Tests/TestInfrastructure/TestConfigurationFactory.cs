using System.IO;
using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    internal class TestConfigurationFactory
    {
        public static IConfiguration Build()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            return configuration;
        }
    }
}