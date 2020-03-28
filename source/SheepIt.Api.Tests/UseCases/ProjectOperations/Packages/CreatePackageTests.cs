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
        private const string _projectId = "foo";
        private int _testEnvironmentId;
        private int _prodEnvironmentId;
        private int _frontendComponentId;

        [SetUp]
        public async Task set_up()
        {
            await Fixture
                .CreateProject(_projectId)
                .WithEnvironmentNames("test", "prod")
                .WithComponents("frontend", "backend")
                .Create();
            
            _testEnvironmentId = await Fixture.FindEnvironmentId("test");
            _prodEnvironmentId = await Fixture.FindEnvironmentId("prod");
            _frontendComponentId = await Fixture.FindComponentId("frontend");
            
            Fixture.MomentLater();
        }

        [Test]
        public async Task can_create_a_package()
        {
            // given
            
            var createPackageResponse = await Fixture.CreatePackage(_projectId)
                .WithDescription("a package")
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_testEnvironmentId, "var-1-test"},
                            {_prodEnvironmentId, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_testEnvironmentId, "var-2-test"},
                            {_prodEnvironmentId, "var-2-prod"}
                        }
                    }
                })
                .Create();

            // when

            var packageDetailsResponse = await Fixture.Handle(new GetPackageDetailsRequest
            {
                ProjectId = _projectId,
                PackageId = createPackageResponse.CreatedPackageId 
            });
            
            // then

            packageDetailsResponse.Should().BeEquivalentTo(new GetPackageDetailsResponse
            {
                Id = createPackageResponse.CreatedPackageId,
                Description = "a package",
                CreatedAt = Fixture.GetUtcNow(),
                
                ProjectId = _projectId,
                
                ComponentId = _frontendComponentId,
                ComponentName = "frontend",
                
                Variables = new []
                {
                    new GetPackageDetailsResponse.VariableDto
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_testEnvironmentId, "var-1-test"},
                            {_prodEnvironmentId, "var-1-prod"}
                        }
                    },
                    new GetPackageDetailsResponse.VariableDto
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {_testEnvironmentId, "var-2-test"},
                            {_prodEnvironmentId, "var-2-prod"}
                        }
                    }
                }
            });
        }
    
        [Test]
        public async Task can_create_a_package_with_updated_variables()
        {
            // given
            
            await Fixture.CreatePackage("foo")
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {1, "var-1-test"},
                            {2, "var-1-prod"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-2",
                        DefaultValue = "var-2-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {1, "var-2-test"},
                            {2, "var-2-prod"}
                        }
                    }
                })
                .Create();
            
            // when

            var createPackageResponse = await Fixture.CreatePackage("foo")
                .WithVariables(new []
                {
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "var-1",
                        DefaultValue = "var-1-updated-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {1, "var-1-test-updated-value"},
                            {2, "var-1-prod-updated-value"}
                        }
                    },
                    new CreatePackageRequest.UpdateVariable
                    {
                        Name = "new-var",
                        DefaultValue = "new-var-default",
                        EnvironmentValues = new Dictionary<int, string>
                        {
                            {1, "new-var-test"},
                            {2, "new-var-prod"}
                        }
                    }
                })
                .Create();

            // then
            
            var response = await Fixture.Handle(new GetPackageDetailsRequest
            {
                ProjectId = _projectId,
                PackageId = createPackageResponse.CreatedPackageId
            });

            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetPackageDetailsResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "var-1-updated-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "var-1-test-updated-value"},
                        {2, "var-1-prod-updated-value"}
                    }
                },
                new GetPackageDetailsResponse.VariableDto
                {
                    Name = "new-var",
                    DefaultValue = "new-var-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "new-var-test"},
                        {2, "new-var-prod"}
                    }
                }
            });
        }
    }
}