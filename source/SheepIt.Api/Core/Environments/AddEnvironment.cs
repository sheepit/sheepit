using System;
using System.Threading.Tasks;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Environments
{
    public class AddEnvironment
    {
        private readonly SheepItDatabase _database;

        public AddEnvironment(SheepItDatabase database)
        {
            _database = database;
        }
        
        public async Task Add(string projectId, string displayName)
        {
            var environment = new Environment
            {
                Id = await _database.Environments.GetNextId(),
                ProjectId = projectId,
                DisplayName = displayName,
                Rank = await GetNextRank(projectId)
            };

            await _database.Environments
                .InsertOneAsync(environment);
        }

        private async Task<int> GetNextRank(string projectId)
        {
            var environmentsCount = await _database.Environments
                .Find(filter => filter.FromProject(projectId))
                .CountDocumentsAsync();

            return (int) environmentsCount + 1;
        }
    }
}