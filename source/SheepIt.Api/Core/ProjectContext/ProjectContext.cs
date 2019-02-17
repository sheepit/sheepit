using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;

namespace SheepIt.Api.Core.ProjectContext
{
    public interface IProjectContext
    {
        Project Project { get; }
        Environment[] Environments { get; }
    }
    
    public class ProjectContext : IProjectContext
    {
        public Project Project { get; }
        public Environment[] Environments { get; } // todo: [rt] encapsulate, e. g. with immutable collection

        public ProjectContext(Project project, Environment[] environments)
        {
            Project = project;
            Environments = environments;
        }
    }
}