using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.UseCases;
using SheepIt.Domain;

namespace SheepIt.Api.Tests
{
    public class SampleIntegrationTests
    {
        [Test]
        public async Task can_run_some_handler()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<SheepItModule>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            builder.RegisterInstance(configuration)
                .As<IConfiguration>()
                .SingleInstance();
            
            using (var container = builder.Build())
            {
                var sheepItDatabase = container.Resolve<SheepItDatabase>();

                var databaseName = sheepItDatabase.MongoDatabase.DatabaseNamespace.DatabaseName;
                
                await sheepItDatabase.MongoClient.DropDatabaseAsync(databaseName);

                var handlerMediator = container.Resolve<HandlerMediator>();

                await handlerMediator.Handle(new CreateProjectRequest
                {
                    ProjectId = "foo",
                    RepositoryUrl = "c:\\sheep-it\\sample-process",
                    EnvironmentNames = new[] {"dev", "test", "prod"}
                });

                var projects = await handlerMediator.Handle(new ListProjectsRequest());
                
                projects.Projects.Single().Should().BeEquivalentTo(new ListProjectsResponse.ProjectDto
                {
                    Id = "foo",
                    RepositoryUrl = "c:\\sheep-it\\sample-process"
                });
            }
        }
    }
}