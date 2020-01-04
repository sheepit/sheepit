using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Tests.FeatureObjects;
using SheepIt.Api.Tests.TestInfrastructure;

namespace SheepIt.Api.Tests.Core.ProjectContext
{
    public class ProjectContextTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public void cannot_create_context_of_nonexistent_project()
        {
            // given
            
            var nonexistentProjectId = "foo";
            
            using var dbContextScope = Fixture.BeginDbContextScope();
            
            var projectContextFactory = dbContextScope.Resolve<IProjectContextFactory>();
            
            // when
            
            Func<Task<IProjectContext>> creatingProjectContext = async () => await projectContextFactory.Create(nonexistentProjectId);
            
            // then

            creatingProjectContext.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Contain(nonexistentProjectId);
        }

        [Test]
        public async Task can_get_project_context()
        {
            // given
            
            var projectId = "foo";
            var environments = new string[] { "test", "prod" };

            await Fixture.CreateProject(projectId)
                .WithEnvironmentNames(environments)
                .Create();
            
            // when

            using var dbContextScope = Fixture.BeginDbContextScope();

            var projectContext = await dbContextScope
                .Resolve<IProjectContextFactory>()
                .Create(projectId);
            
            // then

            projectContext.Project.Id.Should().Be(projectId);

            projectContext.Environments
                .Select(environment => environment.DisplayName)
                .Should().Equal(environments);

            // todo: [rt] check other properties? or they should be unit tested?
        }
        
        // todo: [rt] can run handler in context of project
    }
}