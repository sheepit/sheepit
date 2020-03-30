using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Model.Components;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    public static class EntityIdFinder
    {
        public static async Task<int> FindEnvironmentId(this IntegrationTestsFixture fixture, string environmentName)
        {
            using var scope = fixture.BeginDbContextScope();

            var dbContext = scope.Resolve<SheepItDbContext>();

            return await dbContext.Environments
                .Where(environment => environment.DisplayName == environmentName)
                .Select(environment => environment.Id)
                .SingleAsync();
        }
        
        public static async Task<int> FindComponentId(this IntegrationTestsFixture fixture, string componentName)
        {
            using var scope = fixture.BeginDbContextScope();

            var dbContext = scope.Resolve<SheepItDbContext>();

            return await dbContext.Components
                .Where(component => component.Name == componentName)
                .Select(component => component.Id)
                .SingleAsync();
        }

        public static async Task<int> FindProjectsDefaultComponentId(this IntegrationTestsFixture fixture, string projectId)
        {
            using var scope = fixture.BeginDbContextScope();

            var dbContext = scope.Resolve<SheepItDbContext>();

            return await dbContext.Components
                .FromProject(projectId)
                .OrderBy(component => component.Id)
                .Select(package => package.Id)
                .FirstAsync();
        }
    }
}