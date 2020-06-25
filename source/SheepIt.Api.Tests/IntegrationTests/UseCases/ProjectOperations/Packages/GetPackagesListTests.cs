using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.IntegrationTests.FeatureObjects;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api.Tests.IntegrationTests.UseCases.ProjectOperations.Packages
{
    public class GetPackagesListTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task can_get_packages()
        {
            // given
            
            var projectCreationTime = Fixture.GetUtcNow();

            var project = await Fixture.CreateProject()
                .WithComponents("frontend")
                .Create();

            Fixture.MomentLater();

            var firstPackageCreationTime = Fixture.GetUtcNow();
            
            var firstPackage = await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();

            Fixture.MomentLater();

            var secondPackageCreationTime = Fixture.GetUtcNow();

            var secondPackage = await Fixture.CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();

            // when

            var response = await Fixture.Handle(new GetPackagesListRequest
            {
                ProjectId = project.Id
            });
            
            // then

            response.Packages
                .Should().BeEquivalentTo(new[]
                {
                    new GetPackagesListResponse.PackageDto
                    {
                        Id = secondPackage.Id,
                        Description = secondPackage.Description,
                        CreatedAt = secondPackageCreationTime,
                        ComponentId = project.FirstComponent.Id,
                        ComponentName = project.FirstComponent.Name
                    },
                    new GetPackagesListResponse.PackageDto
                    {
                        Id = firstPackage.Id,
                        Description = firstPackage.Description,
                        CreatedAt = firstPackageCreationTime,
                        ComponentId = project.FirstComponent.Id,
                        ComponentName = project.FirstComponent.Name
                    },
                    new GetPackagesListResponse.PackageDto
                    {
                        Id = 1,
                        Description = $"{project.FirstComponent.Name} - initial package",
                        CreatedAt = projectCreationTime,
                        ComponentId = project.FirstComponent.Id,
                        ComponentName = project.FirstComponent.Name
                    }
                });
        }
    }
}