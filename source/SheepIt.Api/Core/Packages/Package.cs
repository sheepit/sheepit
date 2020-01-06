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