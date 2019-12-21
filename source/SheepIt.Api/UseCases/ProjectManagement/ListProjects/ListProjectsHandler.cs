using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
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
        private readonly GetProjectsListQuery _getProjectsListQuery;

        public ListProjectsHandler(GetProjectsListQuery getProjectsListQuery)
        {
            _getProjectsListQuery = getProjectsListQuery;
        }

        public async Task<ListProjectsResponse> Handle(ListProjectsRequest request)
        {
            var projects = await _getProjectsListQuery
                .Get();

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
                Id = project.Id
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