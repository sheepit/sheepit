using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;

namespace SheepIt.Api.Core.ProjectContext
{
    public interface IProjectContext
    {
        // todo: [ts] Basically we are using a domain object here, I'm not sure if it is a good idea
        // imho we should introduce some dto for this purpose
        Project Project { get; }
        // todo: [ts] Same doubts here - propably we shouldn't be using the domain object here
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