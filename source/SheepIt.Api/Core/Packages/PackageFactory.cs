using System.Threading.Tasks;
using SheepIt.Api.DataAccess;
using SheepIt.Api.DataAccess.Sequencing;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.Core.Packages
{
    public class PackageFactory
    {
        private readonly IdStorage _idStorage;
        private readonly IClock _clock;
        private readonly SheepItDbContext _dbContext;

        public PackageFactory(
            IdStorage idStorage,
            IClock clock,
            SheepItDbContext dbContext)
        {
            _idStorage = idStorage;
            _clock = clock;
            _dbContext = dbContext;
        }

        public async Task<Package> CreatePackage(
            string projectId,
            int deploymentProcessId,
            string description,
            VariableCollection variableCollection)
        {
            var firstPackage = new Package
            {
                Id = await _idStorage.GetNext(IdSequence.Package),
                
                DeploymentProcessId = deploymentProcessId,
                ProjectId = projectId,

                Description = description,
                Variables = variableCollection,
                CreatedAt = _clock.UtcNow
            };

            _dbContext.Add(firstPackage);
            
            return firstPackage;
        }
    }
}