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
        
        // data

        public string Name { get; set; }
    }
}