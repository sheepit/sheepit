using System.Threading.Tasks;
using SheepIt.Api.DataAccess.Sequencing;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.Core.Packages
{
    public class PackageFactory
    {
        private readonly IdStorage _idStorage;
        private readonly IClock _clock;

        public PackageFactory(
            IdStorage idStorage,
            IClock clock)
        {
            _idStorage = idStorage;
            _clock = clock;
        }

        public async Task<Package> Create(
            string projectId,
            int deploymentProcessId,
            string description,
            VariableCollection variableCollection)
        {
            return new Package
            {
                Id = await _idStorage.GetNext(IdSequence.Package),
                
                DeploymentProcessId = deploymentProcessId,
                ProjectId = projectId,

                Description = description,
                Variables = variableCollection,
                CreatedAt = _clock.UtcNow
            };
        }
    }
}