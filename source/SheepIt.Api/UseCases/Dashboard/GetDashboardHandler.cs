﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.UseCases.Dashboard
{
    public class GetDashboardRequest : IRequest<GetDashboardResponse>
    {
    }

    public class GetDashboardResponse
    {
        public DeploymentDto[] LastDeployments { get; set; }

        public class DeploymentDto
        {
            public int DeploymentId { get; set; }
            public string ProjectId { get; set; }
            public DateTime DeployedAt { get; set; }
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            public string Status { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetDashboardController : MediatorController
    {
        [HttpGet]
        [Route("dashboard/get-dashboard")]
        public async Task<GetDashboardResponse> ShowDashboard()
        {
            return await Handle(new GetDashboardRequest());
        }
    }

    public class GetDashboardHandler : IHandler<GetDashboardRequest, GetDashboardResponse>
    {
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;
        private readonly SheepItDbContext _dbContext;

        public GetDashboardHandler(
            GetEnvironmentsQuery getEnvironmentsQuery,
            SheepItDbContext dbContext)
        {
            _getEnvironmentsQuery = getEnvironmentsQuery;
            _dbContext = dbContext;
        }

        public async Task<GetDashboardResponse> Handle(GetDashboardRequest options)
        {
            var deployments = await GetLastDeployments();
            var environments = await _getEnvironmentsQuery
                .Get(deployments.Select(x => x.EnvironmentId)); 

            var mappedDeployments = MapDeployments(deployments, environments);
            return new GetDashboardResponse
            {
                LastDeployments = mappedDeployments
            };
        }

        private async Task<List<Deployment>> GetLastDeployments()
        {
            return await _dbContext
                .Deployments
                .OrderByDescending(deployment => deployment.StartedAt)
                .Take(10)
                .ToListAsync();
        }

        private GetDashboardResponse.DeploymentDto[] MapDeployments(
            List<Deployment> deployments,
            List<Environment> environments)
        {
            var mappedDeployments = deployments.Select(deployment => new GetDashboardResponse.DeploymentDto
            {
                DeploymentId = deployment.Id,
                ProjectId = deployment.ProjectId,
                Status = deployment.Status.ToString(),
                EnvironmentId = deployment.EnvironmentId,
                DeployedAt = deployment.StartedAt
            }).ToArray();
            
            foreach (var deployment in mappedDeployments)
            {
                var environment = environments.Single(env => env.Id == deployment.EnvironmentId);
                deployment.EnvironmentDisplayName = environment.DisplayName;
            }

            return mappedDeployments;
        }
    }
    
    public class GetDashboardModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDashboardHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }
}
