using SheepIt.Api.Core.Projects;

namespace SheepIt.Api.Core.ProjectContext
{
    public interface IProjectContext
    {
        Project Project { get; }
    }
    
    public class ProjectContext : IProjectContext
    {
        public Project Project { get; }

        public ProjectContext(Project project)
        {
            Project = project;
        }
    }
}