using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Core.Environments.Queries
{
    public class GetEnvironmentsQuery
    {
        private readonly SheepItDbContext _dbContext;

        public GetEnvironmentsQuery(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Environment>> GetOrderedByRank(string projectId)
        {
            return _dbContext
                .Environments
                .Where(environment => environment.ProjectId == projectId)
                .OrderBy(environment => environment.Rank)
                .ToListAsync();
        }
        
        public Task<List<Environment>> Get(IEnumerable<int> environmentIds)
        {
            return _dbContext.Environments
                .Where(x => environmentIds.Contains(x.Id))
                .ToListAsync();
        }
    }
}