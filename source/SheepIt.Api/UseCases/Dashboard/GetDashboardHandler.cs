using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
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
        private readonly SheepItDatabase _database;
        private readonly IProjectContext _projectContext;

        public GetDashboardHandler(SheepItDatabase database, IProjectContext projectContext)
        {
            _database = database;
            _projectContext = projectContext;
        }

        public async Task<GetDashboardResponse> Handle(GetDashboardRequest options)
        {
            var deployments = await GetLastDeployments();
            var environments = await GetEnvironmentsByIds(deployments.Select(x => x.EnvironmentId));

            var mappedDeployments = MapDeployments(deployments, environments);
            return new GetDashboardResponse
            {
                LastDeployments = mappedDeployments
            };
        }

        private async Task<List<Environment>> GetEnvironmentsByIds(IEnumerable<int> environmentIds)
        {
            return await _database
                .Environments
                .Find(x => x.In(field => field.Id, environmentIds))
                .ToListAsync();
        }

        private async Task<List<Deployment>> GetLastDeployments()
        {
            return await _database
                .Deployments
                .Find(x => x.Empty)
                .Sort(x => x.Descending(y => y.DeployedAt))
                .Limit(10)
                .ToListAsync();
        }

        private GetDashboardResponse.DeploymentDto[] MapDeployments(List<Deployment> deployments, List<Environment> environments)
        {
            var mappedDeployments = deployments.Select(deployment => new GetDashboardResponse.DeploymentDto
            {
                DeploymentId = deployment.Id,
                ProjectId = deployment.ProjectId,
                Status = deployment.Status.ToString(),
                EnvironmentId = deployment.EnvironmentId,
                DeployedAt = deployment.DeployedAt
            }).ToArray();
            
            foreach (var deployment in mappedDeployments)
            {
                var environment = environments.SingleOrDefault(env => env.Id == deployment.EnvironmentId);
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
