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

        [Obsolete("use async version")]
        public Project GetSync(string projectId)
        {
            return Get(projectId).Result;
        }

        public async Task<Project> Get(string projectId)
        {
            return await _database.Projects
                .FindById(projectId);
        }
    }
}