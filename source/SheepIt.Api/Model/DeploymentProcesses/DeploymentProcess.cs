using System.Collections.Generic;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.Model.DeploymentProcesses
{
    public class DeploymentProcess
    {
        // identity
        
        public int Id { get; set; }
        
        // relations

        public int ComponentId { get; set; }
        public virtual Component Component { get; set; }
        
        public virtual List<Package> Packages { get; set; }
        
        // data
        
        public byte[] File { get; set; }
    }
}