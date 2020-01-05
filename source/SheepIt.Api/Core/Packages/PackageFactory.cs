using System.Collections.Generic;
using System.Threading.Tasks;
using SheepIt.Api.DataAccess.Sequencing;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.Core.Packages
{
    public class PackageFactory
    {
        private readonly IdStorage _idStorage;
        private readonly IClock _clock;

        public PackageFactory(IdStorage idStorage, IClock clock)
        {
            _idStorage = idStorage;
            _clock = clock;
        }
        
        public async Task<Package> CreateFirstPackage(string projectId, int deploymentProcessId)
        {
            return Package.CreateFirstPackage(
                packageId: await _idStorage.GetNext(IdSequence.Package),
                projectId: projectId,
                createdAt: _clock.UtcNow,
                deploymentProcessId: deploymentProcessId
            );
        }
        
        public async Task<Package> CreatePackageWithUpdatedProperties(
            Package basePackage,
            VariableValues[] newVariables,
            string newDescription,
            int deploymentPackageId)
        {
            var newPackageId = await _idStorage.GetNext(IdSequence.Package);

            return basePackage.CreatePackageWithUpdatedProperties(
                newPackageId: newPackageId,
                createdAt: _clock.UtcNow,
                newVariables: newVariables,
                newDescription: newDescription,
                deploymentPackageId: deploymentPackageId
            );
        }

        public async Task<Package> CreatePackageWithNewVariables(
            Package basePackage,
            IEnumerable<VariableValues> newVariables)
        {
            var newPackageId = await _idStorage.GetNext(IdSequence.Package);

            return basePackage.CreatePackageWithNewVariables(
                newPackageId: newPackageId,
                createdAt: _clock.UtcNow,
                newVariables: newVariables
            );
        }
    }
}