using MongoDB.Bson;
using MongoDB.Driver;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;

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
        public IMongoCollection<DeploymentProcess> DeploymentProcesses => MongoDatabase.GetCollection<DeploymentProcess>();
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