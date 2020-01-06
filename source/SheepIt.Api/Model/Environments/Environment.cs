using System.Collections.Generic;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Projects;

namespace SheepIt.Api.Model.Environments
{
    public class Environment
    {
        // identity
        
        public int Id { get; set; }
        
        // relations
        
        public string ProjectId { get; set; }
        public virtual Project Project { get; set; }
        
        public virtual List<Deployment> Deployments { get; set; }
        
        // data
        
        public string DisplayName { get; set; }
        public int Rank { get; set; }
    }
}