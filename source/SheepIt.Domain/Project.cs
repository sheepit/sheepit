using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SheepIt.Domain
{
    public class Project : IDocumentWithId<string>
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public string Id { get; set; }
        public string RepositoryUrl { get; set; }
    }

    public static class Projects
    {
        private static readonly SheepItDatabase _database = new SheepItDatabase();
        
        public static void Add(Project project)
        {
            _database.Projects
                .InsertOne(project);
        }

        public static Project Get(string projectId)
        {
            return _database.Projects
                .FindById(projectId);
        }
    }
}