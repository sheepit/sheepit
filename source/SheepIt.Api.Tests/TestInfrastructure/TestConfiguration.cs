using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    public class TestConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            builder.RegisterInstance(configuration)
                .As<IConfiguration>()
                .SingleInstance();
        }
    }
}