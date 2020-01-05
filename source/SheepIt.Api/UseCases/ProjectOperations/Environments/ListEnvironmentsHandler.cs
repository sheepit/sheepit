using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

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
    
    [Route("api")]
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

    public class ListEnvironmentsHandler : IHandler<ListEnvironmentsRequest, ListEnvironmentsResponse>
    {
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;

        public ListEnvironmentsHandler(
            GetEnvironmentsQuery getEnvironmentsQuery)
        {
            _getEnvironmentsQuery = getEnvironmentsQuery;
        }

        public async Task<ListEnvironmentsResponse> Handle(ListEnvironmentsRequest request)
        {
            var environments = await _getEnvironmentsQuery
                .GetOrderedByRank(request.ProjectId);

            return new ListEnvironmentsResponse
            {
                Environments = environments
                    .Select(Map)
                    .ToArray()
            };
        }

        private ListEnvironmentsResponse.EnvironmentDto Map(Environment environment)
        {
            return new ListEnvironmentsResponse.EnvironmentDto
            {
                Id = environment.Id,
                DisplayName = environment.DisplayName
            };
        }
    }
    
    public class ListEnvironmentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListEnvironmentsHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}
