using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.Infrastructure.Mongo
{
    public class SheepItDatabase
    {
        public MongoClient MongoClient { get; }
        public IMongoDatabase MongoDatabase { get; }

        public SheepItDatabase(MongoSettings settings)
        {
            MongoClient = new MongoClient(settings.ConnectionString);
            MongoDatabase = MongoClient.GetDatabase(name: settings.DatabaseName);
        }

        public IMongoCollection<Environment> Environments => MongoDatabase.GetCollection<Environment>();
        public IMongoCollection<Project> Projects => MongoDatabase.GetCollection<Project>();
        public IMongoCollection<Deployment> Deployments => MongoDatabase.GetCollection<Deployment>();
        public IMongoCollection<Release> Releases => MongoDatabase.GetCollection<Release>();

        public async Task InitializeCounters()
        {
            var name = "Counter";
            
            var collections = await MongoDatabase.ListCollectionNamesAsync();
            var list = await collections.ToListAsync();
            if (list.Any(x => x == name))
                return;
            
            MongoDatabase.CreateCollection(name);

            var counterCollection = MongoDatabase.GetCollection<Counter>();
            await counterCollection.InsertOneAsync(new Counter
            {
                Id = "EnvironmentId",
                Value = 0
            });
        }

        public async Task<int> GetNextSequence(string name)
        {
            var coll = MongoDatabase.GetCollection<Counter>();
            var counter = await coll.FindOneAndUpdateAsync(x => x.Id == name, Builders<Counter>.Update.Inc(x => x.Value, 1));
            return counter.Value;
        }
    }


    public class Counter : IDocumentWithId<string>
    {
        public string Id { get; set; }
        public int Value { get; set; }
        public ObjectId ObjectId { get; }
    }

    public interface IDocumentInProject
    {
        string ProjectId { get; }
    }

    public interface IDocumentWithId<out TId> : IDocument
    {
        TId Id { get; }
    }

    public interface IDocument
    {
        ObjectId ObjectId { get; }
    }
    
    
}