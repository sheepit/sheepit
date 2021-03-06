using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.ProjectContext;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class GetPackageDetailsRequest : IRequest<GetPackageDetailsResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int PackageId { get; set; }
    }

    public class GetPackageDetailsResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ProjectId { get; set; }
        
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        
        public VariableDto[] Variables { get; set; }

        public class VariableDto
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    [Route("frontendApi")]
    [ApiController]
    public class GetPackageDetailsController : MediatorController
    {
        [HttpPost]
        [Route("project/package/get-package-details")]
        public async Task<GetPackageDetailsResponse> GetPackageDetails(GetPackageDetailsRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetPackageDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetPackageDetailsHandler>()
                .InProjectLock()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetPackageDetailsHandler : IHandler<GetPackageDetailsRequest, GetPackageDetailsResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetPackageDetailsHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetPackageDetailsResponse> Handle(GetPackageDetailsRequest request)
        {
            var foundPackage = await _dbContext.Packages
                .Include(package => package.Component)
                .FindByIdAndProjectId(
                    packageId: request.PackageId,
                    projectId: request.ProjectId
                );
            
            return new GetPackageDetailsResponse
            {
                Id = foundPackage.Id,
                Description = foundPackage.Description,
                CreatedAt = foundPackage.CreatedAt,
                
                ComponentId = foundPackage.ComponentId,
                ComponentName = foundPackage.Component.Name,
                
                ProjectId = foundPackage.ProjectId,
                
                Variables = foundPackage.Variables.Variables
                    .Select(values => new GetPackageDetailsResponse.VariableDto
                    {
                        Name = values.Name,
                        DefaultValue = values.DefaultValue,
                        EnvironmentValues = values
                            .GetEnvironmentValues()
                            .ToDictionary(
                                pair => pair.Key,
                                pair => pair.Value
                            )
                    })
                    .ToArray()
            };
        }
    }
}
