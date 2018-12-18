using System;
using System.Linq;

namespace SheepIt.Domain
{
    public class Deployment
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public int ReleaseId { get; set; }
        public DateTime DeployedAt { get; set; } // todo: started at?
        public string EnvironmentId { get; set; }
        public DeploymentStatus Status { get; set; }
    }

    public enum DeploymentStatus
    {
        InProgress,
        Succeeded,
        Failed
    }

    public static class Deployments
    {
        public static int Add(Deployment deployment)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Deployment>();

                return collection.Insert(deployment);
            }
        }

        public static Deployment[] GetAll(string projectId)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Deployment>();

                return collection
                    .Find(deployment => deployment.ProjectId == projectId)
                    .ToArray();
            }
        }

        public static void ChangeDeploymentStatus(int deploymentId, DeploymentStatus newStatus)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Deployment>();

                var deployment = collection.FindById(deploymentId);

                deployment.Status = newStatus;

                collection.Update(deployment);
            }
        }
    }
}