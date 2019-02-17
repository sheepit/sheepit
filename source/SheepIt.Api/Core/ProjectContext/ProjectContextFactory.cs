using System.Threading.Tasks;
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

        public ProjectContextFactory(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<IProjectContext> Create(string projectId)
        {
            var project = _database.Projects.FindById(projectId);

            var environments = _database.Environments
                .Find(filter => filter.FromProject(projectId))
                .ToArray();

            return new ProjectContext(project, environments);
        }
    }
}