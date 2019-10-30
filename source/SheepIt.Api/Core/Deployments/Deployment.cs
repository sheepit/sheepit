using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Deployments
{
    public class Deployment : IDocumentWithId<int>, IDocumentInProject
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public int PackageId { get; set; }
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

    public static class DeploymentFilters
    {
        public static FilterDefinition<Deployment> OfPackage(this FilterDefinitionBuilder<Deployment> filter, int packageId)
        {
            return filter.Eq(deployment => deployment.PackageId, packageId);
        }
    }
}