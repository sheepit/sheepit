using System;
using System.Collections.Generic;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Projects;

namespace SheepIt.Api.Model.Packages
{
    public class Package
    {
        // identity
        
        public int Id { get; set; }
        
        // relations
        
        public string ProjectId { get; set; }
        public virtual Project Project { get; set; }
        
        public int DeploymentProcessId { get; set; }
        public virtual DeploymentProcess DeploymentProcess { get; set; }

        public virtual List<Deployment> Deployments { get; set; }
        
        // data
        
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public VariableCollection Variables { get; set; }

        public VariableForEnvironment[] GetVariablesForEnvironment(int environmentId)
        {
            return Variables.GetForEnvironment(environmentId);
        }
    }
}