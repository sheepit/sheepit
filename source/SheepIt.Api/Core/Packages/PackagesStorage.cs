using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;

namespace SheepIt.Api.Core.Packages
{
    public class PackagesStorage
    {
        private readonly SheepItDbContext _dbContext;
        private readonly IdStorage _idStorage;

        public PackagesStorage(SheepItDbContext dbContext, IdStorage idStorage)
        {
            _dbContext = dbContext;
            _idStorage = idStorage;
        }

        public async Task<int> Add(Package package)
        {
            var nextId = await _idStorage.GetNext(typeof(Package));
            
            package.Id = nextId;

            _dbContext.Packages.Add(package);

            return nextId;
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