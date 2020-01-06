using System.Collections.Generic;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Packages;
using Environment = SheepIt.Api.Model.Environments.Environment;

namespace SheepIt.Api.Model.Projects
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