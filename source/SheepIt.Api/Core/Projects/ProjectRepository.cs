using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Core.Projects
{
    public class ProjectRepository
    {
        private readonly SheepItDbContext _dbContext;

        public ProjectRepository(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Project> Get(Guid objectId)
        {
            var projectOrNull = await _dbContext
                .Projects
                .FirstOrDefaultAsync(project => project.ObjectId == objectId);
            
            if (projectOrNull == null)
            {
                throw new InvalidOperationException($"Project with objectId {objectId} was not found.");
            }
            
            return projectOrNull;
        }
        
        public async Task<Project> Get(string id)
        {
            var projectOrNull = await TryGet(id);

            if (projectOrNull == null)
            {
                throw new InvalidOperationException($"Project with id {id} was not found.");
            }
            
            return projectOrNull;
        }

        public async Task<Project> TryGet(string id)
        {
            return await _dbContext
                .Projects
                .FirstOrDefaultAsync(project => project.Id == id);
        }

        public Project Create(Project project)
        {
            return _dbContext.Projects.Add(project).Entity;
        }

        public async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}