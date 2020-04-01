using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Deployments;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public class GetDeploymentsListRequest : IRequest<GetDeploymentsListResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class GetDeploymentsListResponse
    {
        public DeploymentDto[] Deployments { get; set; }
        
        public class DeploymentDto
        {
            public int Id { get; set; }
            public string Status { get; set; }
            public DateTime DeployedAt { get; set; } // todo: [rt] started at
            
            public int PackageId { get; set; }
            public string PackageDescription { get; set; }
            
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            
            public int ComponentId { get; set; }
            public string ComponentName { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetDeploymentsListController : MediatorController
    {
        [HttpPost]
        [Route("project/deployments/list-deployments")]
        public async Task<GetDeploymentsListResponse> ShowDashboard(GetDeploymentsListRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetDeploymentsListModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDeploymentsListHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetDeploymentsListHandler : IHandler<GetDeploymentsListRequest, GetDeploymentsListResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetDeploymentsListHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetDeploymentsListResponse> Handle(GetDeploymentsListRequest request)
        {
            return new GetDeploymentsListResponse
            {
                Deployments = await GetDeployments(request.ProjectId)
            };
        }

        private async Task<GetDeploymentsListResponse.DeploymentDto[]> GetDeployments(string projectId)
        {
            return await _dbContext.Deployments
                .FromProject(projectId)
                .OrderByNewest()
                .Select(deployment => new GetDeploymentsListResponse.DeploymentDto
                {
                    Id = deployment.Id,
                    EnvironmentId = deployment.Environment.Id,
                    EnvironmentDisplayName = deployment.Environment.DisplayName,
                    DeployedAt = deployment.StartedAt,
                    PackageId = deployment.PackageId,
                    PackageDescription = deployment.Package.Description,
                    Status = deployment.Status.ToString(),
                    ComponentId = deployment.Package.ComponentId,
                    ComponentName = deployment.Package.Component.Name
                })
                .ToArrayAsync();
        }
    }
}
