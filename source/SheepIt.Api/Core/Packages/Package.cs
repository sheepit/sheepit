using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Packages
{
    public class Package : IDocumentWithId<int>, IDocumentInProject
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public int DeploymentProcessId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public VariableCollection Variables { get; set; } = new VariableCollection();

        public VariableForEnvironment[] GetVariablesForEnvironment(int environmentId)
        {
            return Variables.GetForEnvironment(environmentId);
        }
        
        public Package WithUpdatedDeploymentProcess(int newDeploymentProcessId)
        {
            return new Package
            {
                Id = 0,
                ProjectId = ProjectId,
                DeploymentProcessId = newDeploymentProcessId,
                CreatedAt = DateTime.UtcNow,
                Variables = Variables.Clone()
            };
        }

        public Package WithUpdatedVariables(VariableValues[] newVariables)
        {
            return new Package
            {
                Id = 0,
                ProjectId = ProjectId,
                DeploymentProcessId = DeploymentProcessId,
                CreatedAt = DateTime.UtcNow,
                Variables = Variables.WithUpdatedVariables(newVariables)
            };
        }

        public Package WithNewVariables(IEnumerable<VariableValues> newVariables)
        {
            return new Package
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
