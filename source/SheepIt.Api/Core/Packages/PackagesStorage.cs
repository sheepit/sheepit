using System.Threading.Tasks;
using MongoDB.Driver;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Packages
{
    public class PackagesStorage
    {
        private readonly SheepItDatabase _database;

        public PackagesStorage(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<int> Add(PackageMongoEntity package)
        {
            var nextId = await _database.Packages.GetNextId();
            
            package.Id = nextId;

            await _database.Packages.InsertOneAsync(package);

            return nextId;
        }

        public async Task<PackageMongoEntity> GetNewest(string projectId)
        {
            return await _database.Packages
                .Find(filter => filter.FromProject(projectId))
                .Sort(sort => sort.Descending(package => package.Id))
                .FirstAsync();
        }
    }
}