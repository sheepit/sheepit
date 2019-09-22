using System.Threading.Tasks;
using MongoDB.Bson;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcessStorage
    {
        private readonly SheepItDatabase _database;

        public DeploymentProcessStorage(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<ObjectId> Add(byte[] zipFile)
        {
            var id = ObjectId.GenerateNewId();
            
            await _database.DeploymentProcesses.InsertOneAsync(new DeploymentProcess
            {
                ObjectId = id,
                File = zipFile
            });

            return id;
        }
        
    }
}