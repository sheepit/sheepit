using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public class GetDashboardRequest : IRequest<GetDashboardResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class GetDashboardResponse
    {
        public EnvironmentDto[] Environments { get; set; }
        public DeploymentDto[] Deployments { get; set; }
        public ReleaseDto[] Releases { get; set; }

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
            public int CurrentReleaseId { get; set; }
        }
        
        public class DeploymentDto
        {
            public int Id { get; set; }
            public int ReleaseId { get; set; }
            public DateTime DeployedAt { get; set; }
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            public string Status { get; set; }
        }

        public class ReleaseDto
        {
            public int Id { get; set; }
            public string CommitSha { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetDashboardController : MediatorController
    {
        [HttpPost]
        [Route("project/dashboard/get-dashboard")]
        public async Task<GetDashboardResponse> ShowDashboard(GetDashboardRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetDashboardHandler : ISyncHandler<GetDashboardRequest, GetDashboardResponse>
    {
        private readonly SheepItDatabase _database;
        private readonly IProjectContext _projectContext;

        public GetDashboardHandler(SheepItDatabase database, IProjectContext projectContext)
        {
            _database = database;
            _projectContext = projectContext;
        }

        public GetDashboardResponse Handle(GetDashboardRequest options)
        {
            var deployments = _database.Deployments
                .Find(filter => filter.FromProject(options.ProjectId))
                .ToArray();

            var releases = _database.Releases
                .Find(filter => filter.FromProject(options.ProjectId))
                .ToArray();

            return new GetDashboardResponse
            {
                Environments = EnvironmentList.GetEnvironments(_projectContext.Environments, deployments),
                Deployments = DeploymentList.GetDeployments(deployments, _projectContext.Environments),
                Releases = ReleaseList.GetReleases(releases)
            };
        }
    }
    
    public class ShowDashboardModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDashboardHandler>()
                .AsAsyncHandler()
                .InProjectContext()
                .RegisterIn(builder);
        }
    }
}
