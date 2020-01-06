using System.Threading.Tasks;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcessFactory
    {
        private readonly IdStorage _idStorage;
        private readonly ValidateZipFile _validateZipFile;

        public DeploymentProcessFactory(IdStorage idStorage, ValidateZipFile validateZipFile)
        {
            _idStorage = idStorage;
            _validateZipFile = validateZipFile;
        }

        public async Task<DeploymentProcess> Create(string projectId, byte[] zipFileBytes)
        {
            _validateZipFile.Validate(zipFileBytes);
            
            return new DeploymentProcess
            {
                Id = await _idStorage.GetNext(IdSequence.DeploymentProcess),
                ProjectId = projectId,
                File = zipFileBytes
            };
        }
    }
}