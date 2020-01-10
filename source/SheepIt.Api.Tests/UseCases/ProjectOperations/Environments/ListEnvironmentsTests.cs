using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Environments;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Environments
{
    public class ListEnvironmentsTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_list_project_environments()
        {
            // given
            
            await Fixture.CreateProject("foo")
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();
            
            // when

            var response = await Fixture.Handle(new ListEnvironmentsRequest
            {
                ProjectId = "foo"
            });

            // then

            response.Environments
                .Select(environment => environment.DisplayName)
                .Should().Equal("dev", "test", "prod");
        }
    }
}