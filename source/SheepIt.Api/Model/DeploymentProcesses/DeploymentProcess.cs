using System.Collections.Generic;
using SheepIt.Api.Model.Packages;
using SheepIt.Api.Model.Projects;

namespace SheepIt.Api.Model.DeploymentProcesses
{
    public class DeploymentProcess
    {
        // identity
        
        public int Id { get; set; }
        
        // relations
        
        public string ProjectId { get; set; }
        public virtual Project Project { get; set; }
        
        public virtual List<Package> Packages { get; set; }
        
        // data
        
        public byte[] File { get; set; }
    }
}