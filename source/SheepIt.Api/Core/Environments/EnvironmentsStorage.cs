using System;
using System.Threading.Tasks;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Environments
{
    public class EnvironmentsStorage
    {
        private readonly SheepItDatabase _database;

        public EnvironmentsStorage(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task Add(Environment environment)
        {
            // todo: we should set rank and id when creating document, not when saving it, IMO
            environment.SetRank(GetNextRank(environment));
            environment.Id = await _database.Environments.GetNextId();
            
            await _database.Environments
                .InsertOneAsync(environment);
        }

        private int GetNextRank(Environment environment)
        {
            var environmentsCount = _database.Environments
                .Find(filter => filter.FromProject(environment.ProjectId))
                .CountDocuments();

            return (int) environmentsCount + 1;
        }

        public void Update(Environment environment)
        {
            _database.Environments
                .ReplaceOneByIdSync(environment);
        }
    }
}