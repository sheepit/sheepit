using System.Threading.Tasks;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Deployments
{
    public class DeploymentsStorage
    {
        private readonly SheepItDatabase _database;

        public DeploymentsStorage(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<int> Add(Deployment deployment)
        {
            var nextId = await _database.Deployments.GetNextId();
            
            deployment.Id = nextId;

            await _database.Deployments
                .InsertOneAsync(deployment);

            return nextId;
        }
    }
}