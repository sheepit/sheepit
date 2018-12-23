using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet]
        [Route("list-projects")]
        public object ListProjects()
        {
            return ListProjectsHandler.Handle(new ListProjectsRequest());
        }
    }

    public static class ListProjectsHandler
    {
        public static ListProjectsResponse Handle(ListProjectsRequest options)
        {
            using (var database = Database.Open())
            {
                var projectCollection = database.GetCollection<Project>();

                var projects = projectCollection
                    .FindAll()
                    .OrderBy(deployment => deployment.Id)
                    .Select(Map)
                    .ToArray();

                return new ListProjectsResponse
                {
                    Projects = projects
                };
            }
        }

        private static ListProjectsResponse.ProjectDto Map(Project project)
        {
            return new ListProjectsResponse.ProjectDto
            {
                Id = project.Id,
                RepositoryUrl = project.RepositoryUrl
            };
        }
    }
}