﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class ListPackageDeploymentsRequest : IRequest<ListPackageDeploymentsResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int PackageId { get; set; }
    }

    public class ListPackageDeploymentsResponse
    {
        public DeploymentDto[] Deployments { get; set; }
        
        public class DeploymentDto
        {
            public int Id { get; set; }
            public int PackageId { get; set; }
            public DateTime DeployedAt { get; set; }
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            public string Status { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class ListPackageDeploymentsController : MediatorController
    {
        [HttpPost]
        [Route("project/package/list-deployments")]
        public async Task<ListPackageDeploymentsResponse> ListDeployments(ListPackageDeploymentsRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListPackageDeploymentsHandler : IHandler<ListPackageDeploymentsRequest, ListPackageDeploymentsResponse>
    {
        private readonly SheepItDatabase sheepItDatabase;
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;

        public ListPackageDeploymentsHandler(
            SheepItDatabase sheepItDatabase,
            GetEnvironmentsQuery getEnvironmentsQuery)
        {
            this.sheepItDatabase = sheepItDatabase;
            _getEnvironmentsQuery = getEnvironmentsQuery;
        }

        public async Task<ListPackageDeploymentsResponse> Handle(ListPackageDeploymentsRequest options)
        {
            var deployments = await sheepItDatabase.Deployments
                .Find(filter => filter.And(
                    filter.FromProject(options.ProjectId),
                    filter.OfPackage(options.PackageId)
                ))
                .SortBy(deployment => deployment.DeployedAt)
                .ToArray();

            var environments = await _getEnvironmentsQuery
                .Get(options.ProjectId);
            
            var deploymentDtos = deployments.Join(
                inner: environments,
                outerKeySelector: deployment => deployment.EnvironmentId,
                innerKeySelector: environment => environment.Id,
                resultSelector: (deployment, environment) => new ListPackageDeploymentsResponse.DeploymentDto
                {
                    Id = deployment.Id,
                    EnvironmentId = environment.Id,
                    EnvironmentDisplayName = environment.DisplayName,
                    DeployedAt = deployment.DeployedAt,
                    PackageId = deployment.PackageId,
                    Status = deployment.Status.ToString()
                }
            );

            return new ListPackageDeploymentsResponse
            {
                Deployments = deploymentDtos.ToArray()
            };
        }
    }
    
    public class ListPackageDeploymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListPackageDeploymentsHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}
