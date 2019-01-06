using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SheepIt.Domain
{
    public class Environment : IDocumentWithId<int>
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
        public static void Add(Environment environment)
        {
            var environmentsForProject = GetAll(environment.ProjectId);

            environment.SetRank(environmentsForProject.Length + 1);
            
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Environment>();

                collection.InsertWithIntId(environment);
            }
        }

        public static Environment Get(int environmentId)
        {
            using (var database = Database.Open())
            {
                var environmentCollection = database.GetCollection<Environment>();

                return environmentCollection.FindById(environmentId);
            }
        }

        public static Environment[] GetAll(string projectId)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Environment>();

                return collection
                    .Find(environment => environment.ProjectId == projectId)
                    .OrderBy(environment => environment.Rank)
                    .ToArray();
            }
        }
        
        public static void Update(Environment environment)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Environment>();

                collection.Update(environment);
            }
        }
    }
}