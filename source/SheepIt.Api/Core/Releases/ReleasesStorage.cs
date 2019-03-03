using System;
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

        [Obsolete("use async version")]
        public int AddSync(Release release)
        {
            return Add(release).Result;
        }
        
        public async Task<int> Add(Release release)
        {
            var nextId = await _database.Releases.GetNextId();
            
            release.Id = nextId;

            await _database.Releases.InsertOneAsync(release);

            return nextId;
        }

        [Obsolete("use async version")]
        public Release GetNewestSync(string projectId)
        {
            return GetNewest(projectId).Result;
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