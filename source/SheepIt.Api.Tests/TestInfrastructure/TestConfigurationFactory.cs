using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    internal class TestConfigurationFactory
    {
        public static IConfiguration Build()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configurationBuilder.AddJsonFile("appsettings.linux.json", optional: true);
            }
            
            return configurationBuilder.Build();
        }
    }
}