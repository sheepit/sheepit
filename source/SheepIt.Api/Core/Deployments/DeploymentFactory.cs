using System.Threading.Tasks;
using SheepIt.Api.DataAccess;
using SheepIt.Api.DataAccess.Sequencing;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.Core.Deployments
{
    public class DeploymentFactory
    {
        private readonly IdStorage _idStorage;
        private readonly IClock _clock;

        public DeploymentFactory(
            IdStorage idStorage,
            IClock clock)
        {
            _idStorage = idStorage;
            _clock = clock;
        }

        public async Task<Deployment> Create(string projectId, int environmentId, int packageId)
        {
            return new Deployment
            {
                Id = await _idStorage.GetNext(IdSequence.Deployment),
                
                ProjectId = projectId,
                PackageId = packageId,
                EnvironmentId = environmentId,
                
                StartedAt = _clock.UtcNow,
                Status = DeploymentStatus.InProgress
            };
        }
    }
}