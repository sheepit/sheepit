using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Domain;
using Environment = SheepIt.Domain.Environment;

namespace SheepIt.Api.UseCases.Deployments
{
    public class ListDeploymentsRequest
    {
        public string ProjectId { get; set; }
        public int? ReleaseId { get; set; }
    }

    public class ListDeploymentResponse
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
    public class ListDeploymentsController : ControllerBase
    {
        private readonly ListDeploymentsHandler _handler;

        public ListDeploymentsController(ListDeploymentsHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("list-deployments")]
        public object ListDeployments(ListDeploymentsRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class ListDeploymentsHandler
    {
        private readonly SheepItDatabase sheepItDatabase;
        private readonly Domain.Environments _environments;

        public ListDeploymentsHandler(SheepItDatabase sheepItDatabase, Domain.Environments environments)
        {
            this.sheepItDatabase = sheepItDatabase;
            _environments = environments;
        }

        public ListDeploymentResponse Handle(ListDeploymentsRequest options)
        {
            var deployments = sheepItDatabase.Deployments
                .Find(GetDeploymentFilter(projectId: options.ProjectId, releaseIdOrNull: options.ReleaseId))
                .SortBy(deployment => deployment.DeployedAt)
                .ToArray();
            
            var environments = _environments
                .GetAll(options.ProjectId);

            var deploymentDtos = deployments.Join(
                inner: environments,
                outerKeySelector: deployment => deployment.EnvironmentId,
                innerKeySelector: environment => environment.Id,
                resultSelector: MapDeployment
            );

            return new ListDeploymentResponse
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

        private ListDeploymentResponse.DeploymentDto MapDeployment(Deployment deployment, Environment environment)
        {
            return new ListDeploymentResponse.DeploymentDto
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
}
