using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.ProjectContext;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Environments;

namespace SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails
{
    public class GetProjectDashboardRequest : IRequest<GetProjectDashboardResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }
    
    public class GetProjectDashboardRequestValidator : AbstractValidator<GetProjectDashboardRequest>
    {
        public GetProjectDashboardRequestValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull()
                .MinimumLength(1);
        }
    }


    public class GetProjectDashboardResponse
    {
        public EnvironmentDto[] Environments { get; set; }
        public ComponentDto[] Components { get; set; }
        public DeploymentDto[] Deployments { get; set; }

        public class EnvironmentDto
        {
            public int EnvironmentId { get; set; }
            public string DisplayName { get; set; }
        }
        
        public class ComponentDto
        {
            public int ComponentId { get; set; }
            public string DisplayName { get; set; }
        }

        public class DeploymentDto
        {
            public int ComponentId { get; set; }
            public int EnvironmentId { get; set; }
            public int DeploymentId { get; set; }
            public int PackageId { get; set; }
            public string PackageDescription { get; set; }
            public DateTime StartedAt { get; set; }
        }
    }

    [Route("frontendApi")]
    [ApiController]
    public class GetProjectDashboardController : MediatorController
    {
        [HttpPost]
        [Route("project/dashboard")]
        public async Task<GetProjectDashboardResponse> GetProjectDashboard(GetProjectDashboardRequest request)
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

        public GetProjectDashboardHandler(
            SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetProjectDashboardResponse> Handle(GetProjectDashboardRequest request)
        {
            var projectId = request.ProjectId;
            
            var environments = await _dbContext.Environments
                .FromProject(projectId)
                .OrderByRank()
                .Select(environment => new GetProjectDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = environment.Id,
                    DisplayName = environment.DisplayName,
                })
                .ToArrayAsync();
            
            var components = await _dbContext.Components
                .Where(component => component.ProjectId == projectId)
                .OrderBy(component => component.Rank)
                .Select(component => new GetProjectDashboardResponse.ComponentDto
                {
                    ComponentId = component.Id,
                    DisplayName = component.Name
                })
                .ToArrayAsync();

            var lastDeployments = await new GetProjectDashboardLastDeploymentsQuery(_dbContext)
                .Execute(projectId);

            return new GetProjectDashboardResponse
            {
                Environments = environments,
                Components = components,
                Deployments = lastDeployments.Select(deployment => new GetProjectDashboardResponse.DeploymentDto
                {
                    ComponentId = deployment.ComponentId,
                    EnvironmentId = deployment.EnvironmentId,
                    DeploymentId = deployment.DeploymentId,
                    PackageId  = deployment.PackageId,
                    PackageDescription = deployment.PackageDescription,
                    StartedAt = deployment.StartedAt
                }).ToArray()
            };
        }
    }
}