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

        [Obsolete("use async version")]
        public void AddSync(Environment environment)
        {
            Add(environment).Wait();
        }
        
        public async Task Add(Environment environment)
        {
            // todo: we should set rank and id when creating document, not when saving it, IMO
            environment.SetRank(GetNextRank(environment));
            environment.Id = _database.Environments.GetNextIdSync();
            
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

        public Environment Get(int environmentId)
        {
            // todo: we need to also check project id, as environment ids will duplicate in the future!
            return _database.Environments
                .FindByIdSync(environmentId);
        }

        public Environment[] GetAll(string projectId)
        {
            return _database.Environments
                .Find(filter => filter.FromProject(projectId))
                .Sort(sort => sort.Ascending(environment => environment.Rank))
                .ToArraySync();
        }
        
        public void Update(Environment environment)
        {
            _database.Environments
                .ReplaceOneByIdSync(environment);
        }
    }
}