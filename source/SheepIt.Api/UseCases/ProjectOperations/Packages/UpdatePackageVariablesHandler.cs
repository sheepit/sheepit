using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class UpdatePackageVariablesRequest : IRequest<UpdatePackageVariablesResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public UpdateVariable[] Updates { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    public class UpdatePackageVariablesResponse
    {
        public int CreatedPackageId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdatePackageVariablesController : MediatorController
    {
        // meant to be used programatically via public API, e. g. when you want to update single variable, like service version
        [HttpPost]
        [Route("project/package/update-package-variables")]
        public async Task<UpdatePackageVariablesResponse> UpdatePackageVariables(UpdatePackageVariablesRequest request)
        {
            return await Handle(request);
        }
    }

    public class UpdatePackageVariablesHandler : IHandler<UpdatePackageVariablesRequest, UpdatePackageVariablesResponse>
    {
        private readonly PackagesStorage _packagesStorage;

        public UpdatePackageVariablesHandler(PackagesStorage packagesStorage)
        {
            _packagesStorage = packagesStorage;
        }

        public async Task<UpdatePackageVariablesResponse> Handle(UpdatePackageVariablesRequest request)
        {
            var package = await _packagesStorage.GetNewest(request.ProjectId);

            var variableValues = request.Updates
                .Select(update => new VariableValues
                {
                    Name = update.Name,
                    DefaultValue = update.DefaultValue,
                    EnvironmentValues = update.EnvironmentValues
                })
                .ToArray();

            var newPackage = package.WithUpdatedVariables(variableValues);

            var newPackageId = await _packagesStorage.Add(newPackage);

            return new UpdatePackageVariablesResponse
            {
                CreatedPackageId = newPackageId
            };
        }
    }
    
    public class UpdatePackageVariablesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdatePackageVariablesHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}