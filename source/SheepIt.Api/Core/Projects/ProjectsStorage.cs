using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Projects
{
    public class ProjectsStorage
    {
        private readonly SheepItDatabase _database;

        public ProjectsStorage(SheepItDatabase database)
        {
            _database = database;
        }

        public void Add(Project project)
        {
            _database.Projects
                .InsertOne(project);
        }

        public Project Get(string projectId)
        {
            return _database.Projects
                .FindById(projectId);
        }
    }
}