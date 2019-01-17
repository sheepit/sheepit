using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases
{
    public class ListProjectsRequest
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
    public class ListProjectsController : ControllerBase
    {
        private readonly ListProjectsHandler _handler;

        public ListProjectsController(ListProjectsHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        [Route("list-projects")]
        public object ListProjects()
        {
            return _handler.Handle(new ListProjectsRequest());
        }
    }

    public class ListProjectsHandler
    {
        private readonly SheepItDatabase sheepItDatabase;

        public ListProjectsHandler(SheepItDatabase sheepItDatabase)
        {
            this.sheepItDatabase = sheepItDatabase;
        }

        public ListProjectsResponse Handle(ListProjectsRequest options)
        {
            var projects = sheepItDatabase.Projects
                .FindAll()
                .SortBy(deployment => deployment.Id)
                .ToEnumerable()
                .Select(Map)
                .ToArray();

            return new ListProjectsResponse
            {
                Projects = projects
            };
        }

        private ListProjectsResponse.ProjectDto Map(Project project)
        {
            return new ListProjectsResponse.ProjectDto
            {
                Id = project.Id,
                RepositoryUrl = project.RepositoryUrl
            };
        }
    }
}