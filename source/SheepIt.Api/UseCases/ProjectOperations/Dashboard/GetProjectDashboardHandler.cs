﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public class GetProjectDashboardRequest : IRequest<GetProjectDashboardResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class GetProjectDashboardResponse
    {
        public EnvironmentDto[] Environments { get; set; }
        public DeploymentDto[] Deployments { get; set; }
        public PackageDto[] Packages { get; set; }

        public class EnvironmentDto
        {
            public int EnvironmentId { get; set; }
            public string DisplayName { get; set; }
            public EnvironmentDeploymentDto Deployment { get; set; }
        }
        
        public class EnvironmentDeploymentDto
        {
            public DateTime LastDeployedAt { get; set; }
            public int CurrentDeploymentId { get; set; }
            public int CurrentPackageId { get; set; }
            public string CurrentPackageDescription { get; set; }
        }
        
        public class DeploymentDto
        {
            public int Id { get; set; }
            public int PackageId { get; set; }
            public string PackageDescription { get; set; }
            public DateTime DeployedAt { get; set; } // todo: [rt] started at
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            public string Status { get; set; }
        }

        public class PackageDto
        {
            public int Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Description { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetProjectDashboardController : MediatorController
    {
        [HttpPost]
        [Route("project/dashboard/get-dashboard")]
        public async Task<GetProjectDashboardResponse> ShowDashboard(GetProjectDashboardRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetProjectDashboardHandler : IHandler<GetProjectDashboardRequest, GetProjectDashboardResponse>
    {
        private readonly IProjectContext _projectContext;
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;
        private readonly SheepItDbContext _dbContext;

        public GetProjectDashboardHandler(
            IProjectContext projectContext,
            GetEnvironmentsQuery getEnvironmentsQuery,
            SheepItDbContext dbContext)
        {
            _projectContext = projectContext;
            _getEnvironmentsQuery = getEnvironmentsQuery;
            _dbContext = dbContext;
        }

        public async Task<GetProjectDashboardResponse> Handle(GetProjectDashboardRequest options)
        {
            var deployments = await _dbContext.Deployments
                .Where(deployment => deployment.ProjectId == options.ProjectId)
                .ToArrayAsync();

            var packages = await _dbContext.Packages
                .Where(package => package.ProjectId == options.ProjectId)
                .ToArrayAsync();

            var environments = await _getEnvironmentsQuery
                .GetOrderedByRank(options.ProjectId);
            
            return new GetProjectDashboardResponse
            {
                Environments = EnvironmentList.GetEnvironments(
                    environments.ToArray(), deployments, packages),
                
                Deployments = DeploymentList.GetDeployments(
                    deployments, _projectContext.Environments, packages),
                
                Packages = PackageList.GetPackages(packages)
            };
        }
    }
    
    public class GetProjectDashboardModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetProjectDashboardHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}
