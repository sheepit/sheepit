using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Tests.IntegrationTests.FeatureObjects;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Packages;
using CreatePackageFeature = SheepIt.Api.Tests.IntegrationTests.PublicApi.FeatureObjects.CreatePackageFeature;
using CreatePackageRequest = SheepIt.Api.PublicApi.Packages.CreatePackageRequest;

namespace SheepIt.Api.Tests.IntegrationTests.PublicApi.Packages
{
    public class CreatePackageHandlerTests : Test<IntegrationTestsFixture>
    {
        private CreateProjectFeature.CreatedProject _project;
        
        private CreateProjectFeature.CreatedEnvironment _test => _project.FirstEnvironment;
        private CreateProjectFeature.CreatedEnvironment _prod => _project.SecondEnvironment;

        [SetUp]
        public async Task set_up()
        {
            _project = await Fixture.CreateProject()
                .WithEnvironmentNames("test", "prod")
                .Create();

            Fixture.MomentLater();
        }
        
        [Test]
        public async Task can_create_a_package()
        {
            // when
            
            var createdPackage = await CreatePackageFeature.CreatePackage(Fixture, _project.Id, _project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<string, string>
                        {
                            {_project.FirstEnvironment.Id.ToString(), "var-1-test"},
                            {_project.SecondEnvironment.Id.ToString(), "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<string, string>
                        {
                            {_project.FirstEnvironment.Id.ToString(), "var-2-test"},
                            {_project.SecondEnvironment.Id.ToString(), "var-2-prod"}
                        }
                    }
                })
                .Create();

            // then

            var packageDetailsResponse = await Fixture.Handle(new GetPackageDetailsRequest
            {
                ProjectId = _project.Id,
                PackageId = createdPackage.Id 
            });
            
            packageDetailsResponse.Should().BeEquivalentTo(new GetPackageDetailsResponse
            {
                Id = createdPackage.Id,
                Description = createdPackage.Description,
                CreatedAt = Fixture.GetUtcNow(),
                
                ProjectId = _project.Id,
                
                ComponentId = _project.FirstComponent.Id,
                ComponentName = _project.FirstComponent.Name,
                
                Variables = new []
                {
                    new GetPackageDetailsResponse.VariableDto
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_test.Id, "var-1-test"},
                            {_prod.Id, "var-1-prod"}
                        }
                    },
                    new GetPackageDetailsResponse.VariableDto
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_test.Id, "var-2-test"},
                            {_prod.Id, "var-2-prod"}
                        }
                    }
                }
            });
        }
    
        [Test]
        public async Task can_create_a_package_with_updated_variables()
        {
            // given
            
            await CreatePackageFeature.CreatePackage(Fixture, _project.Id, _project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<string, string>
                        {
                            {_test.Id.ToString(), "var-1-test"},
                            {_prod.Id.ToString(), "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<string, string>
                        {
                            {_test.Id.ToString(), "var-2-test"},
                            {_prod.Id.ToString(), "var-2-prod"}
                        }
                    }
                })
                .Create();
            
            // when

            var createdPackage = await CreatePackageFeature.CreatePackage(Fixture, _project.Id, _project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-updated-default",
                        EnvironmentValues = new Dictionary<string, string>
                        {
                            {_test.Id.ToString(), "var-1-test-updated-value"},
                            {_prod.Id.ToString(), "var-1-prod-updated-value"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "new-var",
                        DefaultValue = "new-var-default",
                        EnvironmentValues = new Dictionary<string, string>
                        {
                            {_test.Id.ToString(), "new-var-test"},
                            {_prod.Id.ToString(), "new-var-prod"}
                        }
                    }
                })
                .Create();

            // then
            
            var response = await Fixture.Handle(new GetPackageDetailsRequest
            {
                ProjectId = _project.Id,
                PackageId = createdPackage.Id
            });

            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetPackageDetailsResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "var-1-updated-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {_test.Id, "var-1-test-updated-value"},
                        {_prod.Id, "var-1-prod-updated-value"}
                    }
                },
                new GetPackageDetailsResponse.VariableDto
                {
                    Name = "var-2",
                    DefaultValue = "var-2-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {_test.Id, "var-2-test"},
                        {_prod.Id, "var-2-prod"}
                    }
                },
                new GetPackageDetailsResponse.VariableDto
                {
                    Name = "new-var",
                    DefaultValue = "new-var-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {_test.Id, "new-var-test"},
                        {_prod.Id, "new-var-prod"}
                    }
                }
            });
        }

        [Test]
        public async Task when_no_deployment_process_zip_is_specified_previous_one_will_be_used()
        {
            // given
            
            var previousPackage = await CreatePackageFeature.CreatePackage(Fixture, _project.Id, _project.FirstComponent.Id)
                .Create();
            
            // when
            
            var newPackage = await CreatePackageFeature.CreatePackage(Fixture, _project.Id, _project.FirstComponent.Id)
                .WithoutDeploymentProcessZip()
                .Create();

            // then

            using var scope = Fixture.BeginDbContextScope();

            var dbContext = scope.Resolve<SheepItDbContext>();

            var previousDeploymentProcessId = await GetDeploymentProcessId(previousPackage.Id);
            var newDeploymentProcessId = await GetDeploymentProcessId(newPackage.Id);

            newDeploymentProcessId.Should().Be(previousDeploymentProcessId);
            
            async Task<int> GetDeploymentProcessId(int packageId)
            {
                return await dbContext.Packages
                    .WithId(packageId)
                    .Select(package => package.DeploymentProcessId)
                    .FirstAsync();
            }
        }
    }
}