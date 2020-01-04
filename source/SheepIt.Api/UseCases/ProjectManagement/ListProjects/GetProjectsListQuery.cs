using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class GetProjectsListQuery
    {
        private readonly SheepItDbContext _dbContext;

        public GetProjectsListQuery(
            SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public Task<List<Project>> Get()
        {
            var query = _dbContext
                .Projects
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .ToListAsync();

            return query;
        }
    }
}