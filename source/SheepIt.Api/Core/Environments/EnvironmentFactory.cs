using System.Threading.Tasks;
using SheepIt.Api.DataAccess;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.Core.Environments
{
    public class EnvironmentFactory
    {
        private readonly IdStorage _idStorage;
        private readonly SheepItDbContext _dbContext;

        public EnvironmentFactory(
            IdStorage idStorage,
            SheepItDbContext dbContext)
        {
            _idStorage = idStorage;
            _dbContext = dbContext;
        }

        public async Task<Environment> Create(string projectId, int rank, string displayName)
        {
            var environment = new Environment
            {
                Id = await _idStorage.GetNext(IdSequence.Environment),
                ProjectId = projectId,
                Rank = rank,
                DisplayName = displayName
            };

            _dbContext.Environments.Add(environment);
            
            return environment;

        }
    }
}