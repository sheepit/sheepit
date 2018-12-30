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
        public int EnvironmentId { get; set; }
        public DeploymentStatus Status { get; set; }
        public ProcessOutput ProcessOutput { get; set; }

        public void MarkFinished(ProcessOutput processOutput)
        {
            ProcessOutput = processOutput;

            Status = processOutput.Steps.All(result => result.Successful)
                ? DeploymentStatus.Succeeded
                : DeploymentStatus.ProcessFailed;
        }

        public void MarkExecutionFailed()
        {
            Status = DeploymentStatus.ExecutionFailed;
        }
    }

    public enum DeploymentStatus
    {
        InProgress,
        Succeeded,
        ProcessFailed,
        ExecutionFailed
    }
    
    public class ProcessOutput
    {
        public ProcessStepResult[] Steps{ get; set; }
    }

    public class ProcessStepResult
    {
        public bool Successful { get; set; }
        public string Command { get; set; }
        public string[] Output { get; set; }
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

        public static void Update(Deployment deployment)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Deployment>();

                collection.Update(deployment);
            }
        }

        public static Deployment Get(string projectId, int deploymentId)
        {
            using (var database = Database.Open())
            {
                var collection = database.GetCollection<Deployment>();

                return collection
                    .Find(deployment => deployment.ProjectId == projectId && deployment.Id == deploymentId)
                    .Single();
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
    }
}