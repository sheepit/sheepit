using System.Threading.Tasks;
using SheepIt.Api.DataAccess;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.Core.DeploymentProcesses
{
    public class DeploymentProcessFactory
    {
        private readonly IdStorage _idStorage;
        private readonly ValidateZipFile _validateZipFile;
        private readonly SheepItDbContext _dbContext;

        public DeploymentProcessFactory(
            IdStorage idStorage,
            ValidateZipFile validateZipFile,
            SheepItDbContext dbContext)
        {
            _idStorage = idStorage;
            _validateZipFile = validateZipFile;
            _dbContext = dbContext;
        }

        public async Task<DeploymentProcess> Create(string projectId, byte[] zipFileBytes)
        {
            _validateZipFile.Validate(zipFileBytes);

            var deploymentProcess = new DeploymentProcess
            {
                Id = await _idStorage.GetNext(IdSequence.DeploymentProcess),
                ProjectId = projectId,
                File = zipFileBytes
            };

            _dbContext.DeploymentProcesses.Add(deploymentProcess);
            
            return deploymentProcess;
        }
    }
}