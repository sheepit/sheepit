using System;
using System.Threading.Tasks;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcessFactory
    {
        private readonly IdStorage _idStorage;

        public DeploymentProcessFactory(IdStorage idStorage)
        {
            _idStorage = idStorage;
        }

        public async Task<DeploymentProcess> Create(string projectId, byte[] zipFileBytes)
        {
            return new DeploymentProcess
            {
                ObjectId = Guid.NewGuid(),
                Id = await _idStorage.GetNext(IdSequence.DeploymentProcess),
                ProjectId = projectId,
                File = zipFileBytes
            };
        }
    }
}