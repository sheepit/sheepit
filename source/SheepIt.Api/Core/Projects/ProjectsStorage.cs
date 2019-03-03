using System;
using System.Threading.Tasks;
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

        public async Task Add(Project project)
        {
            await _database.Projects
                .InsertOneAsync(project);
        }

        public Project Get(string projectId)
        {
            return _database.Projects
                .FindById(projectId);
        }
        
        public void Update(Project project)
        {
            _database.Projects
                .ReplaceOneById(project);
        }
    }
}