using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Environments;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class ListEnvironmentsRequest : IRequest<ListEnvironmentsResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
    }

    public class ListEnvironmentsResponse
    {
        public EnvironmentDto[] Environments { get; set; }

        public class EnvironmentDto
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
        }
    }
    
    [Route("frontendApi")]
    [ApiController]
    public class ListEnvironmentsController : MediatorController
    {
        [HttpPost]
        [Route("project/environment/list-environments")]
        public async Task<ListEnvironmentsResponse> ListEnvironments(ListEnvironmentsRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListEnvironmentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListEnvironmentsHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class ListEnvironmentsHandler : IHandler<ListEnvironmentsRequest, ListEnvironmentsResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public ListEnvironmentsHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ListEnvironmentsResponse> Handle(ListEnvironmentsRequest request)
        {
            var environments = await _dbContext.Environments
                .FromProject(request.ProjectId)
                .OrderByRank()
                .Select(environment => new ListEnvironmentsResponse.EnvironmentDto
                {
                    Id = environment.Id,
                    DisplayName = environment.DisplayName
                })
                .ToArrayAsync();
            
            return new ListEnvironmentsResponse
            {
                Environments = environments
            };
        }
    }
}
