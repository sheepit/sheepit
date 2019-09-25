using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Releases
{
    public class Release : IDocumentWithId<int>, IDocumentInProject
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public int DeploymentProcessId { get; set; }
        public DateTime CreatedAt { get; set; }
        public VariableCollection Variables { get; set; } = new VariableCollection();

        public VariableForEnvironment[] GetVariablesForEnvironment(int environmentId)
        {
            return Variables.GetForEnvironment(environmentId);
        }
        
        public Release WithUpdatedDeploymentProcess(int newDeploymentProcessId)
        {
            return new Release
            {
                Id = 0,
                ProjectId = ProjectId,
                DeploymentProcessId = newDeploymentProcessId,
                CreatedAt = DateTime.UtcNow,
                Variables = Variables.Clone()
            };
        }

        public Release WithUpdatedVariables(VariableValues[] newVariables)
        {
            return new Release
            {
                Id = 0,
                ProjectId = ProjectId,
                DeploymentProcessId = DeploymentProcessId,
                CreatedAt = DateTime.UtcNow,
                Variables = Variables.WithUpdatedVariables(newVariables)
            };
        }

        public Release WithNewVariables(IEnumerable<VariableValues> newVariables)
        {
            return new Release
            {
                Id = 0,
                ProjectId = ProjectId,
                DeploymentProcessId = DeploymentProcessId,
                CreatedAt = DateTime.UtcNow,
                Variables = new VariableCollection
                {
                    Variables = newVariables.ToArray()
                }
            };
        }
    }
}
