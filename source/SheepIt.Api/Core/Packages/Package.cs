using System;
using System.Collections.Generic;
using System.Linq;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Projects;

namespace SheepIt.Api.Core.Packages
{
    public class Package
    {
        // identity
        
        public int Id { get; private set; }
        
        // relations
        
        public string ProjectId { get; private set; }
        public virtual Project Project { get; set; }
        
        public int DeploymentProcessId { get; private set; }
        public virtual DeploymentProcess DeploymentProcess { get; set; }

        public virtual List<Deployment> Deployments { get; set; }
        
        // data
        
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public VariableCollection Variables { get; private set; }

        // ef needs private ctor
        private Package()
        {
        }

        public static Package CreateFirstPackage(int packageId, string projectId, DateTime createdAt, int deploymentProcessId)
        {
            return new Package
            {
                Id = packageId,
                Variables = new VariableCollection(),
                CreatedAt = createdAt,
                ProjectId = projectId,
                DeploymentProcessId = deploymentProcessId
            };
        }
        
        public VariableForEnvironment[] GetVariablesForEnvironment(int environmentId)
        {
            return Variables.GetForEnvironment(environmentId);
        }
        
        public Package CreatePackageWithUpdatedProperties(
            int newPackageId,
            DateTime createdAt,
            VariableValues[] newVariables,
            string newDescription,
            int deploymentPackageId)
        {
            return new Package
            {
                Id = newPackageId,
                ProjectId = ProjectId,
                DeploymentProcessId = deploymentPackageId,
                Description = newDescription,
                CreatedAt = createdAt,
                Variables = Variables.WithUpdatedVariables(newVariables)
            };
        }

        public Package CreatePackageWithNewVariables(int newPackageId, DateTime createdAt,
            IEnumerable<VariableValues> newVariables)
        {
            return new Package
            {
                Id = newPackageId,
                ProjectId = ProjectId,
                DeploymentProcessId = DeploymentProcessId,
                CreatedAt = createdAt,
                Variables = new VariableCollection(newVariables.ToArray())
            };
        }
    }
}