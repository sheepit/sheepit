using Autofac;
using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Tests.IntegrationTests.TestInfrastructure
{
    public class TestConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configuration = TestConfigurationFactory.Build();

            builder.RegisterInstance(configuration)
                .As<IConfiguration>()
                .SingleInstance();
        }
    }
}