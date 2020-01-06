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

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class ListPackageDeploymentsRequest : IRequest<ListPackageDeploymentsResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int PackageId { get; set; }
    }

    public class ListPackageDeploymentsResponse
    {
        public DeploymentDto[] Deployments { get; set; }
        
        public class DeploymentDto
        {
            public int Id { get; set; }
            public int PackageId { get; set; }
            public DateTime DeployedAt { get; set; }
            public int EnvironmentId { get; set; }
            public string EnvironmentDisplayName { get; set; }
            public string Status { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class ListPackageDeploymentsController : MediatorController
    {
        [HttpPost]
        [Route("project/package/list-deployments")]
        public async Task<ListPackageDeploymentsResponse> ListDeployments(ListPackageDeploymentsRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListPackageDeploymentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListPackageDeploymentsHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class ListPackageDeploymentsHandler : IHandler<ListPackageDeploymentsRequest, ListPackageDeploymentsResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public ListPackageDeploymentsHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ListPackageDeploymentsResponse> Handle(ListPackageDeploymentsRequest request)
        {
            var deployments = await _dbContext.Deployments
                .FromProject(request.ProjectId)
                .OfPackage(request.PackageId)
                .OrderByNewest()
                .Select(deployment => new ListPackageDeploymentsResponse.DeploymentDto
                {
                    Id = deployment.Id,
                    EnvironmentId = deployment.Environment.Id,
                    EnvironmentDisplayName = deployment.Environment.DisplayName,
                    DeployedAt = deployment.StartedAt,
                    PackageId = deployment.PackageId,
                    Status = deployment.Status.ToString()
                })
                .ToArrayAsync();

            return new ListPackageDeploymentsResponse
            {
                Deployments = deployments
            };
        }
    }
}
