using System;
using System.Linq;

namespace SheepIt.ConsolePrototype
{
    public class Deployment
    {
        public int Id { get; set; }
        public string ProjectIt { get; set; }
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

        public static Deployment[] GetAll(ShowDashboardOptions options)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                return deploymentCollection
                    .Find(deployment => deployment.ProjectIt == options.ProjectId)
                    .ToArray();
            }
        }
    }
}