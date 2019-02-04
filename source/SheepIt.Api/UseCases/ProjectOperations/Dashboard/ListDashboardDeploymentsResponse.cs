using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public class ListProjectDeploymentsRequest : IRequest<ListProjectDeploymentsResponse>
    {
        public string ProjectId { get; set; }
    }

    public class ListProjectDeploymentsResponse
    {
        public DeploymentDto[] Deployments { get; set; }
        
        public class DeploymentDto
        {
            public int Id { get; set; }
            public int ReleaseId { get; set; }
            public DateTime DeployedAt { get; set; }
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            public string Status { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class ListProjectDeploymentsController : MediatorController
    {
        [HttpPost]
        [Route("project/dashboard/list-deployments")]
        public async Task<ListProjectDeploymentsResponse> ListProjectDeployments(ListProjectDeploymentsRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListProjectDeploymentsHandler : ISyncHandler<ListProjectDeploymentsRequest, ListProjectDeploymentsResponse>
    {
        private readonly SheepItDatabase sheepItDatabase;

        public ListProjectDeploymentsHandler(SheepItDatabase sheepItDatabase)
        {
            this.sheepItDatabase = sheepItDatabase;
        }

        public ListProjectDeploymentsResponse Handle(ListProjectDeploymentsRequest options)
        {
            var deployments = sheepItDatabase.Deployments
                .Find(filter => filter.FromProject(options.ProjectId))
                .SortBy(deployment => deployment.DeployedAt)
                .ToArray();

            var environments = sheepItDatabase.Environments
                .Find(filter => filter.FromProject(options.ProjectId))
                .ToArray();

            var deploymentDtos = deployments.Join(
                inner: environments,
                outerKeySelector: deployment => deployment.EnvironmentId,
                innerKeySelector: environment => environment.Id,
                resultSelector: (deployment, environment) => new ListProjectDeploymentsResponse.DeploymentDto
                {
                    Id = deployment.Id,
                    EnvironmentId = environment.Id,
                    EnvironmentDisplayName = environment.DisplayName,
                    DeployedAt = deployment.DeployedAt,
                    ReleaseId = deployment.ReleaseId,
                    Status = deployment.Status.ToString()
                }
            );

            return new ListProjectDeploymentsResponse
            {
                Deployments = deploymentDtos.ToArray()
            };
        }
    }
    
    public class ListProjectDeploymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListProjectDeploymentsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}
