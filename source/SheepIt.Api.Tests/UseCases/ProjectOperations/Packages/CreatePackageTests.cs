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

        [SetUp]
        public async Task set_up()
        {
            await Fixture
                .CreateProject(_projectId)
                .WithEnvironmentNames("test", "prod")
                .Create();
        }

        [Test]
        public async Task can_create_a_package()
        {
            // when
            
            await Fixture.CreatePackage("foo")
                .WithDescription("last")
                .Create();
            
            // then
            
            var response = await Fixture.Handle(new GetLastPackageRequest
            {
                ProjectId = _projectId
            });
            
            response.Description.Should().Be("last");
        }

        [Test]
        public async Task can_create_a_package_with_variables()
        {
            // when
            
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
            
            // then
            
            var response = await Fixture.Handle(new GetLastPackageRequest
            {
                ProjectId = _projectId
            });

            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "var-1-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "var-1-test"},
                        {2, "var-1-prod"}
                    }
                },
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-2",
                    DefaultValue = "var-2-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "var-2-test"},
                        {2, "var-2-prod"}
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
            
            await Fixture.CreatePackage("foo")
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
            
            var response = await Fixture.Handle(new GetLastPackageRequest
            {
                ProjectId = _projectId
            });

            response.Variables.Should().BeEquivalentTo(new[]
            {
                new GetLastPackageResponse.VariableDto
                {
                    Name = "var-1",
                    DefaultValue = "var-1-updated-default",
                    EnvironmentValues = new Dictionary<int, string>
                    {
                        {1, "var-1-test-updated-value"},
                        {2, "var-1-prod-updated-value"}
                    }
                },
                new GetLastPackageResponse.VariableDto
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