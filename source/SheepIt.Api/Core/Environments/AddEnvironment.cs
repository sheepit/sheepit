using System.Threading.Tasks;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Core.Environments
{
    public class AddEnvironment
    {
        private readonly SheepItDbContext _dbContext;
        private readonly EnvironmentFactory _environmentFactory;

        public AddEnvironment(
            SheepItDbContext dbContext,
            EnvironmentFactory environmentFactory)
        {
            _dbContext = dbContext;
            _environmentFactory = environmentFactory;
        }

        public async Task AddMany(string projectId, string[] displayNames)
        {
            var currentEnvironmentRank = 1;
            
            foreach (var environmentName in displayNames)
            {
                var environment = await _environmentFactory.Create(
                    projectId: projectId,
                    rank: currentEnvironmentRank,
                    displayName: environmentName
                );

                _dbContext.Environments.Add(environment);

                currentEnvironmentRank++;
            }
        }
    }
}