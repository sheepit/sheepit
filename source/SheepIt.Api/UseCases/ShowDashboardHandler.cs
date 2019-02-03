using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.UseCases
{
    public class ShowDashboardRequest : IRequest<ShowDashboardResponse>
    {
        public string ProjectId { get; set; }
    }

    public class ShowDashboardResponse
    {
        public EnvironmentDto[] Environments { get; set; }

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
    }

    [Route("api")]
    [ApiController]
    public class ShowDashboardController : MediatorController
    {
        [HttpPost]
        [Route("show-dashboard")]
        public async Task<ShowDashboardResponse> ShowDashboard(ShowDashboardRequest request)
        {
            return await Handle(request);
        }
    }

    public class ShowDashboardHandler : ISyncHandler<ShowDashboardRequest, ShowDashboardResponse>
    {
        private readonly Core.Deployments.DeploymentsStorage _deploymentsStorage;
        private readonly Core.Environments.EnvironmentsStorage _environmentsStorage;

        public ShowDashboardHandler(Core.Deployments.DeploymentsStorage deploymentsStorage, Core.Environments.EnvironmentsStorage environmentsStorage)
        {
            _deploymentsStorage = deploymentsStorage;
            _environmentsStorage = environmentsStorage;
        }

        public ShowDashboardResponse Handle(ShowDashboardRequest options)
        {
            var projectEnvironments = GetProjectEnvironments(options.ProjectId);
            var deploymentInfoForEnvironments = GetDeploymentsInfoForEnvironments(options.ProjectId);

            var environments = FillEnvironmentsWithDeploymentDetails(projectEnvironments, deploymentInfoForEnvironments);

            return new ShowDashboardResponse
            {
                Environments = environments
            };
        }

        private Environment[] GetProjectEnvironments(string projectId)
        {
            return _environmentsStorage.GetAll(projectId);
        }

        private ShowDashboardResponse.EnvironmentDto[] GetDeploymentsInfoForEnvironments(string projectId)
        {
            var environments = _deploymentsStorage.GetAll(projectId)
                .GroupBy(deployment => deployment.EnvironmentId)
                .Select(grouping => MapDeployment(
                        environmentId: grouping.Key,
                        deployment: grouping
                            .OrderByDescending(deployment => deployment.DeployedAt)
                            .First()
                    )
                )
                .OrderBy(environment => environment.EnvironmentId)
                .ToArray();

            return environments;
        }

        private ShowDashboardResponse.EnvironmentDto MapDeployment(int environmentId, Deployment deployment)
        {
            return new ShowDashboardResponse.EnvironmentDto
            {
                EnvironmentId = environmentId,
                Deployment = new ShowDashboardResponse.EnvironmentDeploymentDto
                {
                    LastDeployedAt = deployment.DeployedAt,
                    CurrentDeploymentId = deployment.Id,
                    CurrentReleaseId = deployment.ReleaseId                    
                }
            };
        }

        private ShowDashboardResponse.EnvironmentDto[] FillEnvironmentsWithDeploymentDetails(
            Environment[] projectEnvironments,
            ShowDashboardResponse.EnvironmentDto[] deploymentInfoForEnvironments
            )
        {
            var result = new List<ShowDashboardResponse.EnvironmentDto>();
            
            foreach (var projectEnvironment in projectEnvironments)
            {
                var deploymentInfoForEnvironment = deploymentInfoForEnvironments
                    .FirstOrDefault(x => x.EnvironmentId == projectEnvironment.Id);

                var item = new ShowDashboardResponse.EnvironmentDto
                {
                    EnvironmentId = projectEnvironment.Id,
                    DisplayName = projectEnvironment.DisplayName
                };
                
                if (deploymentInfoForEnvironment != null)
                    item.Deployment = deploymentInfoForEnvironment.Deployment;
                
                result.Add(item);
            }

            return result.ToArray();
        }
    }
    
    public class ShowDashboardModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ShowDashboardHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}
