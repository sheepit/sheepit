using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class EditPackageVariablesRequest : IRequest, IProjectRequest
    {
        public string ProjectId { get; set; }
        public VariableDto[] NewVariables { get; set; }
        
        public class VariableDto
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class EditPackageVariablesController : MediatorController
    {
        [HttpPost]
        [Route("project/package/edit-package-variables")]
        public async Task EditPackageVariables(EditPackageVariablesRequest request)
        {
            await Handle(request);
        }
    }

    public class EditPackageVariablesHandler : IHandler<EditPackageVariablesRequest>
    {
        private readonly PackageRepository _packageRepository;
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;

        public EditPackageVariablesHandler(
            PackageRepository packageRepository,
            SheepItDbContext dbContext,
            PackageFactory packageFactory)
        {
            _packageRepository = packageRepository;
            _dbContext = dbContext;
            _packageFactory = packageFactory;
        }

        public async Task Handle(EditPackageVariablesRequest request)
        {
            var package = await _packageRepository.GetNewest(request.ProjectId);
            
            var variableValues = request.NewVariables
                .Select(update => VariableValues.Create(update.Name, update.DefaultValue, update.EnvironmentValues))
                .ToArray();

            var newPackage = _packageFactory.CreatePackageWithNewVariables(
                basePackage: package,
                newVariables: variableValues
            );

            _dbContext.Add(newPackage);
        }
    }
    
    public class EditPackageVariablesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<EditPackageVariablesHandler>()
                .WithDefaultResponse()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}