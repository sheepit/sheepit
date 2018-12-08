using System;

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
        public static int InsertDeployment(Deployment deployment)
        {
            using (var database = Database.Open())
            {
                var deploymentCollection = database.GetCollection<Deployment>();

                var id = deploymentCollection.Insert(deployment);

                return id.AsInt32;
            }
        }
    }
}