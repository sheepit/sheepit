using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
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

    public class ListProjectsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListProjectsHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class ListProjectsHandler : IHandler<ListProjectsRequest, ListProjectsResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public ListProjectsHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ListProjectsResponse> Handle(ListProjectsRequest request)
        {
            var projects = await _dbContext.Projects
                .Select(project => new ListProjectsResponse.ProjectDto
                {
                    Id = project.Id
                })                
                .OrderBy(project => project.Id)
                .ToArrayAsync();

            return new ListProjectsResponse
            {
                Projects = projects
            };
        }
    }
}