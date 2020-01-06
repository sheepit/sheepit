using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.ErrorHandling;

namespace SheepIt.Api.Core.Projects
{
    public class ProjectFactory
    {
        private readonly SheepItDbContext _dbContext;

        public ProjectFactory(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Project> Create(string projectId)
        {
            await ValidateProjectIdUniqueness(projectId);

            return new Project
            {
                Id = projectId
            };
        }
        
        private async Task ValidateProjectIdUniqueness(string projectId)
        {
            var projectExists = await _dbContext.Projects
                .WithId(projectId)
                .AnyAsync();

            if (projectExists)
            {
                throw new CustomException(
                    "CREATE_PROJECT_ID_NOT_UNIQUE",
                    "Project with specified id already exists"
                );
            }
        }
    }
}