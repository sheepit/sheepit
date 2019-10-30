using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
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
        }
        
        public class DeploymentDto
        {
            public int Id { get; set; }
            public int PackageId { get; set; }
            public DateTime DeployedAt { get; set; }
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            public string Status { get; set; }
        }

        public class PackageDto
        {
            public int Id { get; set; }
            public DateTime CreatedAt { get; set; }
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
        private readonly SheepItDatabase _database;
        private readonly IProjectContext _projectContext;

        public GetProjectDashboardHandler(SheepItDatabase database, IProjectContext projectContext)
        {
            _database = database;
            _projectContext = projectContext;
        }

        public async Task<GetProjectDashboardResponse> Handle(GetProjectDashboardRequest options)
        {
            var deployments = await _database.Deployments
                .Find(filter => filter.FromProject(options.ProjectId))
                .ToArray();

            var packages = await _database.Packages
                .Find(filter => filter.FromProject(options.ProjectId))
                .ToArray();

            return new GetProjectDashboardResponse
            {
                Environments = EnvironmentList.GetEnvironments(_projectContext.Environments, deployments),
                Deployments = DeploymentList.GetDeployments(deployments, _projectContext.Environments),
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
