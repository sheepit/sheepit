using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Tests.IntegrationTests.FeatureObjects;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;

namespace SheepIt.Api.Tests.IntegrationTests.UseCases.ProjectOperations.Dashboard
{
    public class GetProjectDashboardRequestTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task Should_Return_Correct_Data_To_Populate_Dashboard()
        {
            // given
            var project = await Fixture.CreateProject()
                .WithComponents("comp1", "comp2")
                .WithEnvironmentNames("test", "prod")
                .Create();

            var createdPackage1 = await Fixture
                .CreatePackage(project.Id, project.FirstComponent.Id)
                .Create();
            
            var deployPackageResponse1 = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = createdPackage1.Id,
                EnvironmentId = project.FirstEnvironment.Id
            });

            Fixture.MomentLater();
            
            var createdPackage2 = await Fixture
                .CreatePackage(project.Id, project.SecondComponent.Id)
                .Create();
            
            var deployPackageResponse2 = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = createdPackage2.Id,
                EnvironmentId = project.SecondEnvironment.Id
            });   
            
            Fixture.MomentLater();
            
            var deployPackageResponse3 = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = createdPackage1.Id,
                EnvironmentId = project.FirstEnvironment.Id
            });
            
            Fixture.MomentLater();
            
            var deployPackageResponse4 = await Fixture.Handle(new DeployPackageRequest
            {
                ProjectId = project.Id,
                PackageId = createdPackage2.Id,
                EnvironmentId = project.SecondEnvironment.Id
            });

            List<Deployment> expectedDeployments;
            using (var scope = Fixture.BeginDbContextScope())
            {
                var expectedIds = new List<int>
                {
                    deployPackageResponse3.CreatedDeploymentId,
                    deployPackageResponse4.CreatedDeploymentId
                };
                var dbContext = scope.Resolve<SheepItDbContext>();
                expectedDeployments = dbContext.Deployments
                    .Include(x => x.Package)
                    .Where(x => expectedIds.Contains(x.Id))
                    .ToList();
            }

            // when
            var getProjectDashboardResponse = await Fixture.Handle(new GetProjectDashboardRequest
            {
                ProjectId = project.Id
            });

            // then
            getProjectDashboardResponse.Should().BeEquivalentTo(new GetProjectDashboardResponse
            {
                Environments = project.Environments.Select(env => new GetProjectDashboardResponse.EnvironmentDto
                    {
                        EnvironmentId = env.Id,
                        DisplayName = env.Name
                    }).ToArray(),

                Components = project.Components.Select(component => new GetProjectDashboardResponse.ComponentDto
                    {
                        ComponentId = component.Id,
                        DisplayName = component.Name
                    }).ToArray(),
                
                Deployments = expectedDeployments.Select(depl => new GetProjectDashboardResponse.DeploymentDto
                    {
                        ComponentId = depl.Package.ComponentId,
                        DeploymentId = depl.Id,
                        EnvironmentId = depl.EnvironmentId,
                        PackageId = depl.PackageId,
                        PackageDescription = depl.Package.Description,
                        StartedAt = depl.StartedAt
                    }).ToArray()
            });
        }
    }
}