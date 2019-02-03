using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.UseCases.Deployments
{
    public class ListDeploymentsRequest : IRequest<ListDeploymentsResponse>
    {
        public string ProjectId { get; set; }
        public int? ReleaseId { get; set; }
    }

    public class ListDeploymentsResponse
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
    public class ListDeploymentsController : MediatorController
    {
        [HttpPost]
        [Route("list-deployments")]
        public async Task<ListDeploymentsResponse> ListDeployments(ListDeploymentsRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListDeploymentsHandler : ISyncHandler<ListDeploymentsRequest, ListDeploymentsResponse>
    {
        private readonly SheepItDatabase sheepItDatabase;
        private readonly Core.Environments.EnvironmentsStorage _environmentsStorage;

        public ListDeploymentsHandler(SheepItDatabase sheepItDatabase, Core.Environments.EnvironmentsStorage environmentsStorage)
        {
            this.sheepItDatabase = sheepItDatabase;
            _environmentsStorage = environmentsStorage;
        }

        public ListDeploymentsResponse Handle(ListDeploymentsRequest options)
        {
            var deployments = sheepItDatabase.Deployments
                .Find(GetDeploymentFilter(projectId: options.ProjectId, releaseIdOrNull: options.ReleaseId))
                .SortBy(deployment => deployment.DeployedAt)
                .ToArray();
            
            var environments = _environmentsStorage
                .GetAll(options.ProjectId);

            var deploymentDtos = deployments.Join(
                inner: environments,
                outerKeySelector: deployment => deployment.EnvironmentId,
                innerKeySelector: environment => environment.Id,
                resultSelector: MapDeployment
            );

            return new ListDeploymentsResponse
            {
                Deployments = deploymentDtos.ToArray()
            };
        }

        private FilterDefinition<Deployment> GetDeploymentFilter(string projectId, int? releaseIdOrNull)
        {
            var deploymentFilters = Builders<Deployment>.Filter;

            return deploymentFilters.And(filters: GetFilters());

            IEnumerable<FilterDefinition<Deployment>> GetFilters()
            {
                yield return deploymentFilters.FromProject(projectId);

                if (releaseIdOrNull.HasValue)
                {
                    yield return deploymentFilters.OfRelease(releaseIdOrNull.Value);
                }
            }
        }

        private ListDeploymentsResponse.DeploymentDto MapDeployment(Deployment deployment, Environment environment)
        {
            return new ListDeploymentsResponse.DeploymentDto
            {
                Id = deployment.Id,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.DeployedAt,
                ReleaseId = deployment.ReleaseId,
                Status = deployment.Status.ToString()
            };
        }
    }
    
    public class ListDeploymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListDeploymentsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}
