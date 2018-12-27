using System.Linq;

namespace SheepIt.Domain
{
    public class Environment
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string DisplayName { get; set; }

        public Environment(string projectId, string displayName)
        {
            ProjectId = projectId;
            DisplayName = displayName;
        }
    }

    public static class Environments
    {
        public static int Add(Environment environment)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Environment>();

                return collection.Insert(environment);
            }
        }

        public static Environment[] GetAll(string projectId)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Environment>();

                return collection
                    .Find(environment => environment.ProjectId == projectId)
                    .ToArray();
            }
        }
    }
}