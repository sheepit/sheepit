using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api.Tests.UseCases.ProjectOperations.Packages
{
    public class CreatePackageTests : Test<IntegrationTestsFixture>
    {
        private CreateProjectFeature.CreatedProject _project;
        
        private CreateProjectFeature.CreatedEnvironment _test => _project.FirstEnvironment;
        private CreateProjectFeature.CreatedEnvironment _prod => _project.SecondEnvironment;

        [SetUp]
        public async Task set_up()
        {
            _project = await Fixture
                .CreateProject("foo")
                .WithEnvironmentNames("test", "prod")
                .WithComponents("frontend", "backend")
                .Create();

            Fixture.MomentLater();
        }
        
        [Test]
        public async Task can_create_a_package()
        {
            // when
            
            var createdPackage = await Fixture.CreatePackage(_project.Id, _project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_project.FirstEnvironment.Id, "var-1-test"},
                            {_project.SecondEnvironment.Id, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_project.FirstEnvironment.Id, "var-2-test"},
                            {_project.SecondEnvironment.Id, "var-2-prod"}
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
            
            await Fixture.CreatePackage(_project.Id, _project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_test.Id, "var-1-test"},
                            {_prod.Id, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_test.Id, "var-2-test"},
                            {_prod.Id, "var-2-prod"}
                        }
                    }
                })
                .Create();
            
            // when

            var createdPackage = await Fixture.CreatePackage(_project.Id, _project.FirstComponent.Id)
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-updated-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_test.Id, "var-1-test-updated-value"},
                            {_prod.Id, "var-1-prod-updated-value"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "new-var",
                        DefaultValue = "new-var-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_test.Id, "new-var-test"},
                            {_prod.Id, "new-var-prod"}
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
    }
}