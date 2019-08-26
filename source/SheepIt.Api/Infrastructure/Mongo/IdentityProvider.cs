using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SheepIt.Api.Infrastructure.Mongo
{
    public class Identity : IDocumentWithId<string>
    {
        public string Id { get; set; }
        public int Value { get; set; }
        public ObjectId ObjectId { get; }
    }

    public class IdentityProvider
    {
        private readonly SheepItDatabase _database;

        public IdentityProvider(SheepItDatabase database)
        {
            _database = database;
        }
        
        public async Task InitializeIdentities(List<string> entityNames)
        {
            var identityCollection = _database.MongoDatabase.GetCollection<Identity>();

            foreach (var entityName in entityNames)
            {
                var filter = Builders<Identity>.Filter.Where(x => x.Id == $"{entityName}Id");

                var identity = new Identity
                {
                    Id = $"{entityName}Id",
                    Value = 0
                };

                var existingIdentity = await identityCollection.Find(filter).SingleOrDefaultAsync();
                if (existingIdentity == null)
                {
                    await identityCollection.InsertOneAsync(identity);
                }
            }
        }

        public async Task<int> GetNextId(string entityName)
        {
            var coll = _database.MongoDatabase.GetCollection<Identity>();

            var filter = Builders<Identity>.Filter.Where(x => x.Id == $"{entityName}Id");
            var builder = Builders<Identity>.Update.Inc(x => x.Value, 1);
            var options = new FindOneAndUpdateOptions<Identity>
            {
                IsUpsert = true
            };
            
            var counter = await coll
                .FindOneAndUpdateAsync(
                    filter,
                    builder,
                    options);

            return counter.Value;
        }
    }
}