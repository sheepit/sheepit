using System.Collections.Generic;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.Deployments;
using SheepIt.Api.Model.Environments;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.Model.Projects
{
    public class Project
    {
        // identity
        
        public string Id { get; set; }
        
        // relations

        public virtual List<Environment> Environments { get; set; }
        public virtual List<Component> Components { get; set; }
        public virtual List<Package> Packages { get; set; }
        public virtual List<Deployment> Deployments { get; set; }
    }
}