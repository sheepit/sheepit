using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Deployments;

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

    public class GetDashboardModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDashboardHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetDashboardHandler : IHandler<GetDashboardRequest, GetDashboardResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetDashboardHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetDashboardResponse> Handle(GetDashboardRequest options)
        {
            var deployments = await _dbContext.Deployments
                .OrderByNewest()
                .Take(10)
                .Select(deployment => new GetDashboardResponse.DeploymentDto
                {
                    DeploymentId = deployment.Id,
                    ProjectId = deployment.ProjectId,
                    Status = deployment.Status.ToString(),
                    EnvironmentId = deployment.EnvironmentId,
                    DeployedAt = deployment.StartedAt,
                    EnvironmentDisplayName = deployment.Environment.DisplayName
                })
                .ToArrayAsync();

            return new GetDashboardResponse
            {
                LastDeployments = deployments
            };
        }
    }
}