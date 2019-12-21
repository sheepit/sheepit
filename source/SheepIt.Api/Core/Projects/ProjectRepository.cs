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
            return await _dbContext
                .Projects
                .FirstOrDefaultAsync(x => x.ObjectId == objectId);
        }
        
        public async Task<Project> Get(string id)
        {
            return await _dbContext
                .Projects
                .FirstOrDefaultAsync(x => x.Id == id);
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