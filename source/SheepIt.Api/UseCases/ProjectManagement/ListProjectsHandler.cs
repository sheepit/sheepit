using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class ListProjectsRequest : IRequest<ListProjectsResponse>
    {
    }

    public class ListProjectsResponse
    {
        public ProjectDto[] Projects { get; set; }

        public class ProjectDto
        {
            public string Id { get; set; }
            public string RepositoryUrl { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class ListProjectsController : MediatorController
    {
        [HttpGet]
        [Route("list-projects")]
        public async Task<ListProjectsResponse> ListProjects()
        {
            return await Handle(new ListProjectsRequest());
        }
    }

    public class ListProjectsHandler : IHandler<ListProjectsRequest, ListProjectsResponse>
    {
        private readonly SheepItDatabase _database;

        public ListProjectsHandler(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<ListProjectsResponse> Handle(ListProjectsRequest request)
        {
            var projects = await _database.Projects
                .FindAll()
                .SortBy(deployment => deployment.Id)
                .ToArray();

            return new ListProjectsResponse
            {
                Projects = projects
                    .Select(MapProject)
                    .ToArray()
            };
        }

        private ListProjectsResponse.ProjectDto MapProject(Project project)
        {
            return new ListProjectsResponse.ProjectDto
            {
                Id = project.Id,
                RepositoryUrl = project.RepositoryUrl
            };
        }
    }
    
    public class ListProjectsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListProjectsHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }
}