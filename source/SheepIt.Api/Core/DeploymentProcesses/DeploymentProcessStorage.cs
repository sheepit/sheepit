using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcessStorage
    {
        private readonly SheepItDatabase _database;
        private readonly IdentityProvider _identityProvider;

        public DeploymentProcessStorage(
            SheepItDatabase database,
            IdentityProvider identityProvider)
        {
            _database = database;
            _identityProvider = identityProvider;
        }

        public async Task<int> Add(string projectId, IFormFile zipFile)
        {
            using (var zipStream = new MemoryStream())
            {
                await zipFile.CopyToAsync(zipStream);

                return await Add(projectId, zipStream.ToArray());
            }
        }

        public async Task<int> Add(string projectId, byte[] zipFile)
        {
            var objectId = ObjectId.GenerateNewId();
            var id = await _identityProvider.GetNextId("DeploymentProcess");
            
            await _database.DeploymentProcesses.InsertOneAsync(new DeploymentProcess
            {
                ObjectId = objectId,
                Id = id,
                ProjectId = projectId,
                File = zipFile
            });

            return id;
        }
    }
}