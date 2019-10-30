using System;
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
    public class GetLastPackageRequest : IRequest<GetLastPackageResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class GetLastPackageResponse
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public VariableDto[] Variables { get; set; }

        public class VariableDto
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetLastPackageController : MediatorController
    {
        // currently used for editing variables
        [HttpPost]
        [Route("project/package/get-last-package")]
        public async Task<GetLastPackageResponse> GetLastPackage(GetLastPackageRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetLastPackageHandler : IHandler<GetLastPackageRequest, GetLastPackageResponse>
    {
        private readonly PackagesStorage _packagesStorage;

        public GetLastPackageHandler(PackagesStorage packagesStorage)
        {
            _packagesStorage = packagesStorage;
        }

        public async Task<GetLastPackageResponse> Handle(GetLastPackageRequest request)
        {
            var package = await _packagesStorage.GetNewest(request.ProjectId);

            return new GetLastPackageResponse
            {
                Id = package.Id,
                ProjectId = package.ProjectId,
                CreatedAt = package.CreatedAt,
                Variables = package.Variables.Variables
                    .Select(values => new GetLastPackageResponse.VariableDto
                    {
                        Name = values.Name,
                        DefaultValue = values.DefaultValue,
                        EnvironmentValues = values.EnvironmentValues.ToDictionary(pair => pair.Key, pair => pair.Value)
                    })
                    .ToArray()
            };
        }
    }
    
    public class GetLastPackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetLastPackageHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}