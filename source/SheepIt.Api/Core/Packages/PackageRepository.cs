using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Core.Packages
{
    public class PackageRepository
    {
        private readonly SheepItDbContext _dbContext;

        public PackageRepository(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Package> GetNewest(string projectId)
        {
            return await _dbContext.Packages
                .Where(package => package.ProjectId == projectId)
                .OrderByDescending(package => package.Id)
                .FirstAsync();
        }
    }
}