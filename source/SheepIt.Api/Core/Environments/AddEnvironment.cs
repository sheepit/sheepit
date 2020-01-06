using System.Threading.Tasks;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.DataAccess;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.Core.Environments
{
    public class AddEnvironment
    {
        private readonly IdStorage _idStorage;
        private readonly SheepItDbContext _dbContext;

        public AddEnvironment(
            IdStorage idStorage,
            SheepItDbContext dbContext)
        {
            _idStorage = idStorage;
            _dbContext = dbContext;
        }

        public async Task AddMany(string projectId, string[] displayNames)
        {
            var currentEnvironmentRank = 1;
            
            foreach (var environmentName in displayNames)
            {
                var environment = new Environment
                {
                    Id = await _idStorage.GetNext(IdSequence.Environment),
                    ProjectId = projectId,
                    DisplayName = environmentName,
                    Rank = currentEnvironmentRank
                };

                _dbContext.Environments.Add(environment);

                currentEnvironmentRank++;
            }
        }
    }
}