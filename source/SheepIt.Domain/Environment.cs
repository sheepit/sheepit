using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SheepIt.Domain
{
    public class Environment : IDocumentWithId<int>, IDocumentInProject
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string DisplayName { get; set; }
        public int Rank { get; set; }

        public Environment()
        {
        }
        
        public Environment(string projectId, string displayName)
        {
            ProjectId = projectId;
            DisplayName = displayName;
        }

        public void SetRank(int rank)
        {
            Rank = rank;
        }
    }

    public static class Environments
    {
        private static readonly SheepItDatabase _database = new SheepItDatabase();
        
        public static void Add(Environment environment)
        {
            // todo: we should set rank and id when creating document, not when saving it, IMO
            environment.SetRank(GetNextRank(environment));
            environment.Id = _database.Environments.GetNextId();
            
            _database.Environments
                .InsertOne(environment);
        }

        private static int GetNextRank(Environment environment)
        {
            var environmentsCount = _database.Environments
                .Find(filter => filter.FromProject(environment.ProjectId))
                .CountDocuments();

            return (int) environmentsCount + 1;
        }

        public static Environment Get(int environmentId)
        {
            // todo: we need to also check project id, as environment ids will duplicate in the future!
            return _database.Environments
                .FindById(environmentId);
        }

        public static Environment[] GetAll(string projectId)
        {
            return _database.Environments
                .Find(filter => filter.FromProject(projectId))
                .Sort(sort => sort.Ascending(environment => environment.Rank))
                .ToArray();
        }
        
        public static void Update(Environment environment)
        {
            _database.Environments
                .ReplaceOneById(environment);
        }
    }
}