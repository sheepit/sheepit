using System.Collections.Generic;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Packages;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.Core.Projects
{
    public class Project
    {
        // identity
        
        public string Id { get; set; }
        
        // relations

        public virtual List<Environment> Environments { get; set; }
        public virtual List<Package> Packages { get; set; }
        public virtual List<Deployment> Deployments { get; set; }
    }
}