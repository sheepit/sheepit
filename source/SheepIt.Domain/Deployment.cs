using System;
using System.Linq;

namespace SheepIt.Domain
{
    public class Deployment
    {
        public int Id { get; set; }
        public string ProjectIt { get; set; } // todo: it should be renamed, but it will break my current db-s
        public int ReleaseId { get; set; }
        public DateTime DeployedAt { get; set; }
        public string EnvironmentId { get; set; }
    }

    public static class Deployments
    {
        public static int Add(Deployment deployment)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                return deploymentCollection.Insert(deployment);
            }
        }

        public static Deployment[] GetAll(string projectId)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                return deploymentCollection
                    .Find(deployment => deployment.ProjectIt == projectId)
                    .ToArray();
            }
        }
    }
}