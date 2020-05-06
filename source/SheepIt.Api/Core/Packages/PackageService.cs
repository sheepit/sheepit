using System.Threading.Tasks;
using SheepIt.Api.Core.Packages.CreatePackage;

namespace SheepIt.Api.Core.Packages
{
    public class PackageService
    {
        private readonly CreatePackageCommandHandler _createPackageCommandHandler;

        public PackageService(
            CreatePackageCommandHandler createPackageCommandHandler)
        {
            _createPackageCommandHandler = createPackageCommandHandler;
        }

        public async Task<int> CreatePackage(CreatePackageCommand command)
        {
            return await _createPackageCommandHandler.Execute(command);
        }
    }
}