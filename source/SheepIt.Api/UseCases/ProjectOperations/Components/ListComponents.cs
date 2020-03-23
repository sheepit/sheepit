using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Components;

namespace SheepIt.Api.UseCases.ProjectOperations.Components
{
    public class ListComponentsRequest : IRequest<ListComponentsResponse>
    {
        public string ProjectId { get; set; }
    }

    public class ListComponentsResponse
    {
        public ComponentDto[] Components { get; set; }
        
        public class ComponentDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
    
    [Route("api")]
    [ApiController]
    public class ListComponentsController : MediatorController
    {
        [HttpGet]
        [Route("list-components")]
        public async Task<ListComponentsResponse> ListProjects(ListComponentsRequest request)
        {
            return await Handle(request);
        }
    }
    
    public class ListComponentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListComponentsHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class ListComponentsHandler : IHandler<ListComponentsRequest, ListComponentsResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public ListComponentsHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ListComponentsResponse> Handle(ListComponentsRequest request)
        {
            var components = await _dbContext.Components
                .FromProject(request.ProjectId)
                .Select(component => new ListComponentsResponse.ComponentDto
                {
                    Id = component.Id,
                    Name = component.Name
                })
                .ToArrayAsync();
            
            return new ListComponentsResponse
            {
                Components = components
            };
        }
    }
}