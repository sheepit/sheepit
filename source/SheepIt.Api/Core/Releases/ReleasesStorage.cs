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

        public int Add(Release release)
        {
            var nextId = _database.Releases.GetNextId();
            
            release.Id = nextId;

            _database.Releases.InsertOne(release);

            return nextId;
        }

        public Release Get(string projectId, int releaseId)
        {
            return _database.Releases
                .FindByProjectAndId(projectId, releaseId);
        }

        public Release GetNewest(string projectId)
        {
            return _database.Releases
                .Find(filter => filter.FromProject(projectId))
                .Sort(sort => sort.Descending(release => release.Id))
                .First();
        }
    }
}