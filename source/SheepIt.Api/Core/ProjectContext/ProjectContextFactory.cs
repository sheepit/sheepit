using System.Threading.Tasks;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.ProjectContext
{
    public interface IProjectContextFactory
    {
        Task<IProjectContext> Create(string projectId);
    }

    public class ProjectContextFactory : IProjectContextFactory
    {
        private readonly SheepItDatabase _database;
        private readonly ProjectRepository _projectRepository;
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;

        public ProjectContextFactory(
            SheepItDatabase database,
            ProjectRepository projectRepository,
            GetEnvironmentsQuery getEnvironmentsQuery)
        {
            _database = database;
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