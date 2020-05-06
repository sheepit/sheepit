using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Packages.CreatePackage;
using SheepIt.Api.PublicApi.Packages.CreatePackage;

namespace SheepIt.Api.PublicApi.Packages
{
    public class PackageController : Controller
    {
        private readonly PackageService _packageService;

        public PackageController(PackageService packageService)
        {
            _packageService = packageService;
        }
        
        [HttpPost("/projects/{projectId}/components/{componentId}/packages")]
        public async Task<IActionResult> CreatePackage(
            [FromRoute] string projectId,
            [FromRoute] int componentId,
            [FromBody] CreatePackageRequest request)
        {
            var command = new CreatePackageCommand
            {
                ProjectId = projectId,
                ComponentId = componentId,
                Description = request.Description,
                ZipFile = request.ZipFile,
                VariableUpdates = request.VariableUpdates
                    .Select(x => new
                        CreatePackageCommand.UpdateVariable {
                            Name = x.Name,
                            DefaultValue = x.DefaultValue,
                            EnvironmentValues = x.EnvironmentValues
                        })
                    .ToArray()
            };

            var createdPackageId = await _packageService.CreatePackage(command);
            
            return Ok(createdPackageId);
        }
    }
}