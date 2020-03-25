using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Packages
{
    public class GetPackagesListTests : Test<IntegrationTestsFixture>
    {
        private string _projectId;
        private DateTime _projectCreationTime;

        [SetUp]
        public async Task set_up()
        {
            _projectId = "foo";

            _projectCreationTime = Fixture.GetUtcNow();

            await Fixture.CreateProject(_projectId)
                .WithEnvironmentNames("dev", "test", "prod")
                .Create();
            
            Fixture.MomentLater();
        }
        
        [Test]
        public async Task can_get_packages()
        {
            // given

            var firstPackageCreationTime = Fixture.GetUtcNow();
            
            var firstPackageDescription = "first package";
            
            var firstPackage = await Fixture.CreatePackage(_projectId)
                .WithDescription(firstPackageDescription)
                .Create();

            Fixture.MomentLater();

            var secondPackageCreationTime = Fixture.GetUtcNow();

            var secondPackageDescription = "second package";
            
            var secondPackage = await Fixture.CreatePackage(_projectId)
                .WithDescription(secondPackageDescription)
                .Create();

            // when

            var response = await Fixture.Handle(new GetPackagesListRequest
            {
                ProjectId = _projectId
            });
            
            // then

            response.Packages
                .Should().BeEquivalentTo(new[]
                {
                    new GetPackagesListResponse.PackageDto
                    {
                        Id = secondPackage.CreatedPackageId,
                        Description = secondPackageDescription,
                        CreatedAt = secondPackageCreationTime,
                        ComponentId = 1,
                        ComponentName = "Default component"
                    },
                    new GetPackagesListResponse.PackageDto
                    {
                        Id = firstPackage.CreatedPackageId,
                        Description = firstPackageDescription,
                        CreatedAt = firstPackageCreationTime,
                        ComponentId = 1,
                        ComponentName = "Default component"
                    },
                    new GetPackagesListResponse.PackageDto
                    {
                        Id = 1,
                        Description = "Initial package",
                        CreatedAt = _projectCreationTime,
                        ComponentId = 1,
                        ComponentName = "Default component"
                    }
                });
        }
    }
}