using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.Core.Projects
{
    public class EnvironmentRepository
    {
        private readonly SheepItDbContext _dbContext;

        public EnvironmentRepository(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Environment> Get(Guid objectId)
        {
            return await _dbContext
                .Environments
                .FirstOrDefaultAsync(x => x.ObjectId == objectId);
        }
        
        public async Task<Environment> Get(int id)
        {
            return await _dbContext
                .Environments
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Environment Add(Environment environment)
        {
            return _dbContext.Environments.Add(environment).Entity;
        }
        
        public Environment Update(Environment environment)
        {
            return _dbContext.Environments.Update(environment).Entity;
        }

        public async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}