using System.Collections.Generic;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Model.Projects;

namespace SheepIt.Api.Model.Components
{
    public class Component
    {
        // identity
        
        public int Id { get; set; }
        
        // relations
        
        public string ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public virtual List<Package> Packages { get; set; }
        public virtual List<DeploymentProcess> DeploymentProcesses { get; set; }

        // data

        public string Name { get; set; }
    }
}