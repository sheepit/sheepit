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
        private readonly SheepItDbContext _dbContext;

        public DeploymentFactory(
            IdStorage idStorage,
            IClock clock,
            SheepItDbContext dbContext)
        {
            _idStorage = idStorage;
            _clock = clock;
            _dbContext = dbContext;
        }

        public async Task<Deployment> Create(string projectId, int environmentId, int packageId)
        {
            var deployment = new Deployment
            {
                Id = await _idStorage.GetNext(IdSequence.Deployment),
                
                ProjectId = projectId,
                PackageId = packageId,
                EnvironmentId = environmentId,
                
                StartedAt = _clock.UtcNow,
                Status = DeploymentStatus.InProgress
            };

            _dbContext.Deployments.Add(deployment);
            
            return deployment;
        }
    }
}