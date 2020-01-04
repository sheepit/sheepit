using System.Threading.Tasks;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.Projects;

namespace SheepIt.Api.Core.ProjectContext
{
    public interface IProjectContextFactory
    {
        Task<IProjectContext> Create(string projectId);
    }

    public class ProjectContextFactory : IProjectContextFactory
    {
        private readonly ProjectRepository _projectRepository;
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;

        public ProjectContextFactory(
            ProjectRepository projectRepository,
            GetEnvironmentsQuery getEnvironmentsQuery)
        {
            _projectRepository = projectRepository;
            _getEnvironmentsQuery = getEnvironmentsQuery;
        }

        public async Task<IProjectContext> Create(string projectId)
        {
            var project = await _projectRepository.Get(projectId);

            var environments = await _getEnvironmentsQuery.Get(projectId);

            return new ProjectContext(project, environments.ToArray());
        }
    }
}