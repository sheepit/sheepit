using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SheepIt.Domain
{
    public class Project : IDocumentWithId<string>
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public string Id { get; set; }
        public string RepositoryUrl { get; set; }
    }

    public class Projects
    {
        private readonly SheepItDatabase _database;

        public Projects(SheepItDatabase database)
        {
            _database = database;
        }

        public void Add(Project project)
        {
            _database.Projects
                .InsertOne(project);
        }

        public Project Get(string projectId)
        {
            return _database.Projects
                .FindById(projectId);
        }
    }
}