using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public Task<List<Environment>> Get(string projectId)
        {
            return _dbContext
                .Environments
                .Where(x => x.ProjectId == projectId)
                .ToListAsync();
        }

        public Task<List<Environment>> GetOrderedByRank(string projectId)
        {
            return _dbContext
                .Environments
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.Rank)
                .ToListAsync();
        }
        
        public Task<List<Environment>> Get(IEnumerable<int> environmentIds)
        {
            return _dbContext.Environments
                .Where(x => environmentIds.Contains(x.Id))
                .ToListAsync();
        }
        
        public async Task<Environment> GetByProjectAndId(string projectId, int environmentId)
        {
            var foundDocumentOrNull = await _dbContext
                .Environments
                .SingleOrDefaultAsync(x => x.ProjectId == projectId && x.Id == environmentId);
            
            if (foundDocumentOrNull == null)
            {
                throw new InvalidOperationException(
                    $"Document of type {typeof(Environment).Name} with id {environmentId} in project {projectId} could not be found.");
            }

            return foundDocumentOrNull;
        }
    }
}