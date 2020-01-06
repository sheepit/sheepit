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
using SheepIt.Api.Model.Environments;
using SheepIt.Api.Model.Packages;

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

    public class GetProjectDashboardModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetProjectDashboardHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetProjectDashboardHandler : IHandler<GetProjectDashboardRequest, GetProjectDashboardResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetProjectDashboardHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetProjectDashboardResponse> Handle(GetProjectDashboardRequest request)
        {
            return new GetProjectDashboardResponse
            {
                Environments = await GetEnvironments(request.ProjectId),
                Deployments = await GetDeployments(request.ProjectId),
                Packages = await GetPackages(request.ProjectId)
            };
        }

        private async Task<GetProjectDashboardResponse.EnvironmentDto[]> GetEnvironments(string projectId)
        {
            var environmentsWithDeployments = await _dbContext.Environments
                .FromProject(projectId)
                .OrderByRank()
                .Select(environment => new
                {
                    EnvironmentId = environment.Id,
                    DisplayName = environment.DisplayName,
                    Deployments = environment.Deployments
                        .OrderByDescending(deployment => deployment.StartedAt)
                        .Take(1)
                        .Select(deployment => new GetProjectDashboardResponse.EnvironmentDeploymentDto
                        {
                            CurrentDeploymentId = deployment.Id,
                            CurrentPackageId = deployment.PackageId,
                            CurrentPackageDescription = deployment.Package.Description,
                            LastDeployedAt = deployment.StartedAt
                        })
                })
                .ToArrayAsync();
            
            // additional in-memory selecting is performed because EF doesn't handle FirstOrDefault
            // in subqueries well - lack of any deployments was mapped into an empty DTO, instead of a null
            return environmentsWithDeployments
                .Select(result => new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = result.EnvironmentId,
                    DisplayName = result.DisplayName,
                    Deployment = result.Deployments.FirstOrDefault()
                })
                .ToArray();
        }

        private async Task<GetProjectDashboardResponse.DeploymentDto[]> GetDeployments(string projectId)
        {
            return await _dbContext.Deployments
                .FromProject(projectId)
                .OrderByNewest()
                .Select(deployment => new GetProjectDashboardResponse.DeploymentDto
                {
                    Id = deployment.Id,
                    EnvironmentId = deployment.Environment.Id,
                    EnvironmentDisplayName = deployment.Environment.DisplayName,
                    DeployedAt = deployment.StartedAt,
                    PackageId = deployment.PackageId,
                    PackageDescription = deployment.Package.Description,
                    Status = deployment.Status.ToString()
                })
                .ToArrayAsync();
        }

        private async Task<GetProjectDashboardResponse.PackageDto[]> GetPackages(string projectId)
        {
            return await _dbContext.Packages
                .FromProject(projectId)
                .OrderByNewest()
                .Select(package => new GetProjectDashboardResponse.PackageDto
                {
                    Id = package.Id,
                    CreatedAt = package.CreatedAt,
                    Description = package.Description
                })
                .ToArrayAsync();
        }
    }
}
