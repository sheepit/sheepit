using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.IntegrationTests.FeatureObjects;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Components;

namespace SheepIt.Api.Tests.IntegrationTests.UseCases.ProjectOperations.Components
{
    public class ListComponentsTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_components_for_update()
        {
            // given

            var project = await Fixture.CreateProject()
                .WithComponents("test", "prod")
                .Create();

            var test = project.FirstComponent;
            var prod = project.SecondComponent;

            // when

            var response = await Fixture.Handle(new ListComponentsRequest
            {
                ProjectId = project.Id
            });

            // then

            response.Components.Should().BeEquivalentTo(new[]
            {
                new ListComponentsResponse.ComponentDto
                {
                    Id = test.Id,
                    Name = test.Name
                },
                new ListComponentsResponse.ComponentDto
                {
                    Id = prod.Id,
                    Name = prod.Name
                }
            }, options => options.WithStrictOrdering());
        }
    }
}