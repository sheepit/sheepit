﻿using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class GetEnvironmentsForUpdateRequest : IRequest<GetEnvironmentsForUpdateResponse>
    {
        public string ProjectId { get; set; }
    }

    public class GetEnvironmentsForUpdateResponse
    {
        public EnvironmentDto[] Environments { get; set; }

        public class EnvironmentDto
        {
            public int EnvironmentId { get; set; }
            public string DisplayName { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetEnvironmentsForUpdateController : MediatorController
    {
        [HttpPost]
        [Route("project/environment/get-environments-for-update")]
        public async Task<GetEnvironmentsForUpdateResponse> GetProjectDetails(GetEnvironmentsForUpdateRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetEnvironmentsForUpdateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetEnvironmentsForUpdateHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetEnvironmentsForUpdateHandler : IHandler<GetEnvironmentsForUpdateRequest, GetEnvironmentsForUpdateResponse>
    {
        private readonly SheepItDatabase _database;

        public GetEnvironmentsForUpdateHandler(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<GetEnvironmentsForUpdateResponse> Handle(GetEnvironmentsForUpdateRequest request)
        {
            var environments = await _database.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .Sort(sort => sort.Ascending(environment => environment.Rank)) // todo: create ByRank extension
                .ToArray();
            
            return new GetEnvironmentsForUpdateResponse
            {
                Environments = environments
                    .Select(MapEnvironment)
                    .ToArray()
            };
        }

        private static GetEnvironmentsForUpdateResponse.EnvironmentDto MapEnvironment(Environment environment)
        {
            return new GetEnvironmentsForUpdateResponse.EnvironmentDto
            {
                DisplayName = environment.DisplayName,
                EnvironmentId = environment.Id
            };
        }
    }
}