using System.Threading.Tasks;
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

        public ProjectContextFactory(
            SheepItDatabase database,
            ProjectRepository projectRepository)
        {
            _database = database;
            _projectRepository = projectRepository;
        }

        public async Task<IProjectContext> Create(string projectId)
        {
            var project = await _projectRepository.Get(projectId);

            var environments = await _database.Environments
                .Find(filter => filter.FromProject(projectId))
                .ToArray();

            return new ProjectContext(project, environments);
        }
    }
}