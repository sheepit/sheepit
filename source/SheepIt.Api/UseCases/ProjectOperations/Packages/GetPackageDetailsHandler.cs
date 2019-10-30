using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

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
    public class GetPackageDetailsController : MediatorController
    {
        [HttpPost]
        [Route("project/package/get-package-details")]
        public async Task<GetPackageDetailsResponse> GetPackageDetails(GetPackageDetailsRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetPackageDetailsHandler : IHandler<GetPackageDetailsRequest, GetPackageDetailsResponse>
    {
        private readonly SheepItDatabase _database;

        public GetPackageDetailsHandler(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<GetPackageDetailsResponse> Handle(GetPackageDetailsRequest request)
        {
            var package = await _database.Packages
                .FindByProjectAndId(request.ProjectId, request.PackageId);

            return new GetPackageDetailsResponse
            {
                Id = package.Id,
                ProjectId = package.ProjectId,
                CreatedAt = package.CreatedAt,
                Variables = package.Variables.Variables
                    .Select(values => new GetPackageDetailsResponse.VariableDto
                    {
                        Name = values.Name,
                        DefaultValue = values.DefaultValue,
                        EnvironmentValues = values.EnvironmentValues.ToDictionary(pair => pair.Key, pair => pair.Value)
                    })
                    .ToArray()
            };
        }
    }

    public class GetPackageDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetPackageDetailsHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}