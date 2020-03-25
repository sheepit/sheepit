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
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.UseCases.ProjectOperations.Dashboard
{
    public class GetPackagesListRequest : IRequest<GetPackagesListResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class GetPackagesListResponse
    {
        public PackageDto[] Packages { get; set; }
        
        public class PackageDto
        {
            public int Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Description { get; set; }
            public int ComponentId { get; set; }
            public string ComponentName { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetPackagesListController : MediatorController
    {
        [HttpPost]
        [Route("project/packages/list-packages")]
        public async Task<GetPackagesListResponse> ShowDashboard(GetPackagesListRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetPackagesListModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetPackagesListHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetPackagesListHandler : IHandler<GetPackagesListRequest, GetPackagesListResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetPackagesListHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetPackagesListResponse> Handle(GetPackagesListRequest request)
        {
            return new GetPackagesListResponse
            {
                Packages = await GetPackages(request.ProjectId)
            };
        }

        private async Task<GetPackagesListResponse.PackageDto[]> GetPackages(string projectId)
        {
            return await _dbContext.Packages
                .FromProject(projectId)
                .OrderByNewest()
                .Select(package => new GetPackagesListResponse.PackageDto
                {
                    Id = package.Id,
                    CreatedAt = package.CreatedAt,
                    Description = package.Description,
                    ComponentId = package.ComponentId,
                    ComponentName = package.Component.Name
                })
                .ToArrayAsync();
        }
    }
}
