using System.Threading.Tasks;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Environments
{
    public class AddEnvironment
    {
        private readonly SheepItDatabase _database;
        private readonly EnvironmentRepository _environmentRepository;
        private readonly IdStorage _idStorage;
        private readonly GetEnvironmentsCountQuery _getEnvironmentsCountQuery;

        public AddEnvironment(
            SheepItDatabase database, 
            EnvironmentRepository environmentRepository,
            IdStorage idStorage,
            GetEnvironmentsCountQuery getEnvironmentsCountQuery)
        {
            _database = database;
            _environmentRepository = environmentRepository;
            _idStorage = idStorage;
            _getEnvironmentsCountQuery = getEnvironmentsCountQuery;
        }
        
        public async Task Add(string projectId, string displayName)
        {
            var id = await _idStorage.GetNext(typeof(Environment));
            
            var environment = new Environment
            {
                Id = id, 
                ProjectId = projectId,
                DisplayName = displayName,
                Rank = await GetNextRank(projectId)
            };

            _environmentRepository.Add(environment);
        }

        private async Task<int> GetNextRank(string projectId)
        {
            var environmentsCount = await _getEnvironmentsCountQuery.Get(projectId); 
            return environmentsCount + 1;
        }
    }
}