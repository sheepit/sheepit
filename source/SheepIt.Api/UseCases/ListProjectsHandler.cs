﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases
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

    public class ListProjectsHandler : ISyncHandler<ListProjectsRequest, ListProjectsResponse>
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