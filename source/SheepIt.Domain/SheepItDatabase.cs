using MongoDB.Bson;
using MongoDB.Driver;

namespace SheepIt.Domain
{
    public class SheepItDatabase
    {
        public IMongoDatabase MongoDatabase { get; }

        public SheepItDatabase()
        {
            var mongoClient = new MongoClient();

            // todo: get name from settings
            MongoDatabase = mongoClient.GetDatabase(name: "sheep-it");
        }

        public IMongoCollection<Environment> Environments => MongoDatabase.GetCollection<Environment>();
        public IMongoCollection<Project> Projects => MongoDatabase.GetCollection<Project>();
        public IMongoCollection<Deployment> Deployments => MongoDatabase.GetCollection<Deployment>();
        public IMongoCollection<Release> Releases => MongoDatabase.GetCollection<Release>();
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