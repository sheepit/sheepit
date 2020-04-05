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
    public class GetComponentsForUpdateRequest : IRequest<GetComponentsForUpdateResponse>
    {
        public string ProjectId { get; set; }
    }

    public class GetComponentsForUpdateResponse
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
    public class GetComponentsForUpdateController : MediatorController
    {
        [HttpPost]
        [Route("project/components")]
        public async Task<GetComponentsForUpdateResponse> GetComponentsForUpdate(GetComponentsForUpdateRequest request)
        {
            return await Handle(request);
        }
    }
    
    public class GetComponentsForUpdateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetComponentsForUpdateHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetComponentsForUpdateHandler : IHandler<GetComponentsForUpdateRequest, GetComponentsForUpdateResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetComponentsForUpdateHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetComponentsForUpdateResponse> Handle(GetComponentsForUpdateRequest request)
        {
            var components = await _dbContext.Components
                .FromProject(request.ProjectId)
                .OrderByRank()
                .Select(component => new GetComponentsForUpdateResponse.ComponentDto
                {
                    Id = component.Id,
                    Name = component.Name
                })
                .ToArrayAsync();
            
            return new GetComponentsForUpdateResponse
            {
                Components = components
            };
        }
    }
}