using System.Threading.Tasks;
using MongoDB.Driver;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Releases
{
    public class ReleasesStorage
    {
        private readonly SheepItDatabase _database;

        public ReleasesStorage(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<int> Add(Release release)
        {
            var nextId = await _database.Releases.GetNextId();
            
            release.Id = nextId;

            await _database.Releases.InsertOneAsync(release);

            return nextId;
        }

        public async Task<Release> GetNewest(string projectId)
        {
            return await _database.Releases
                .Find(filter => filter.FromProject(projectId))
                .Sort(sort => sort.Descending(release => release.Id))
                .FirstAsync();
        }
    }
}