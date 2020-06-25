using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.ProjectContext;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Environments;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class GetEnvironmentsListRequest : IRequest<GetEnvironmentsListResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class GetEnvironmentsListResponse
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
            public int CurrentPackageId { get; set; }
            public string CurrentPackageDescription { get; set; }
        }
    }

    [Route("frontendApi")]
    [ApiController]
    public class GetEnvironmentsListController : MediatorController
    {
        [HttpPost]
        [Route("project/environments/list-environments")]
        public async Task<GetEnvironmentsListResponse> GetEnvironmentsList(GetEnvironmentsListRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetEnvironmentsListModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetEnvironmentsListHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetEnvironmentsListHandler : IHandler<GetEnvironmentsListRequest, GetEnvironmentsListResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetEnvironmentsListHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetEnvironmentsListResponse> Handle(GetEnvironmentsListRequest request)
        {
            return new GetEnvironmentsListResponse
            {
                Environments = await GetEnvironments(request.ProjectId)
            };
        }

        private async Task<GetEnvironmentsListResponse.EnvironmentDto[]> GetEnvironments(string projectId)
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
                        .Select(deployment => new GetEnvironmentsListResponse.EnvironmentDeploymentDto
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
                .Select(result => new GetEnvironmentsListResponse.EnvironmentDto
                {
                    EnvironmentId = result.EnvironmentId,
                    DisplayName = result.DisplayName,
                    Deployment = result.Deployments.FirstOrDefault()
                })
                .ToArray();
        }
    }
}
