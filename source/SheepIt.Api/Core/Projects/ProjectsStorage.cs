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
    }
}