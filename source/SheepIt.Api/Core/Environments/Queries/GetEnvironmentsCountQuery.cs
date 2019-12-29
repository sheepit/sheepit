using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Core.Environments.Queries
{
    public class GetEnvironmentsCountQuery
    {
        private readonly SheepItDbContext _dbContext;

        public GetEnvironmentsCountQuery(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> Get(string projectId)
        {
            return _dbContext.Environments
                .CountAsync(x => x.ProjectId == projectId);
        }
    }
}