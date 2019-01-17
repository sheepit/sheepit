using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace SheepIt.Domain
{
    public class Deployment : IDocumentWithId<int>, IDocumentInProject
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
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

    public class Deployments
    {
        private readonly SheepItDatabase _database;

        public Deployments(SheepItDatabase database)
        {
            _database = database;
        }

        public int Add(Deployment deployment)
        {
            var nextId = _database.Deployments.GetNextId();
            
            deployment.Id = nextId;

            _database.Deployments
                .InsertOne(deployment);

            return nextId;
        }

        public void Update(Deployment deployment)
        {
            _database.Deployments
                .ReplaceOneById(deployment);
        }

        public Deployment Get(string projectId, int deploymentId)
        {
            return _database.Deployments
                .FindByProjectAndId(projectId, deploymentId);
        }

        public Deployment[] GetAll(string projectId)
        {
            return _database.Deployments
                .Find(filter => filter.FromProject(projectId))
                .ToArray();
        }
    }

    public static class DeploymentFilters
    {
        public static FilterDefinition<Deployment> OfRelease(this FilterDefinitionBuilder<Deployment> filter, int releaseId)
        {
            return filter.Eq(deployment => deployment.ReleaseId, releaseId);
        }
    }
}