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

namespace SheepIt.Api.UseCases.ProjectOperations.Releases
{
    public class ListReleaseDeploymentsRequest : IRequest<ListReleaseDeploymentsResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int ReleaseId { get; set; }
    }

    public class ListReleaseDeploymentsResponse
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
    public class ListReleaseDeploymentsController : MediatorController
    {
        [HttpPost]
        [Route("project/release/list-deployments")]
        public async Task<ListReleaseDeploymentsResponse> ListDeployments(ListReleaseDeploymentsRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListReleaseDeploymentsHandler : ISyncHandler<ListReleaseDeploymentsRequest, ListReleaseDeploymentsResponse>
    {
        private readonly SheepItDatabase sheepItDatabase;

        public ListReleaseDeploymentsHandler(SheepItDatabase sheepItDatabase)
        {
            this.sheepItDatabase = sheepItDatabase;
        }

        public ListReleaseDeploymentsResponse Handle(ListReleaseDeploymentsRequest options)
        {
            var deployments = sheepItDatabase.Deployments
                .Find(filter => filter.And(
                    filter.FromProject(options.ProjectId),
                    filter.OfRelease(options.ReleaseId)
                ))
                .SortBy(deployment => deployment.DeployedAt)
                .ToArraySync();
            
            var environments = sheepItDatabase.Environments
                .Find(filter => filter.FromProject(options.ProjectId))
                .ToArraySync();
            
            var deploymentDtos = deployments.Join(
                inner: environments,
                outerKeySelector: deployment => deployment.EnvironmentId,
                innerKeySelector: environment => environment.Id,
                resultSelector: (deployment, environment) => new ListReleaseDeploymentsResponse.DeploymentDto
                {
                    Id = deployment.Id,
                    EnvironmentId = environment.Id,
                    EnvironmentDisplayName = environment.DisplayName,
                    DeployedAt = deployment.DeployedAt,
                    ReleaseId = deployment.ReleaseId,
                    Status = deployment.Status.ToString()
                }
            );

            return new ListReleaseDeploymentsResponse
            {
                Deployments = deploymentDtos.ToArray()
            };
        }
    }
    
    public class ListReleaseDeploymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListReleaseDeploymentsHandler>()
                .AsAsyncHandler()
                .InProjectContext()
                .RegisterIn(builder);
        }
    }
}
