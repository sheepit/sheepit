using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class ListEnvironmentsRequest : IRequest<ListEnvironmentsResponse>
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
        [Route("list-environments")]
        public async Task<ListEnvironmentsResponse> ListEnvironments(ListEnvironmentsRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListEnvironmentsHandler : ISyncHandler<ListEnvironmentsRequest, ListEnvironmentsResponse>
    {
        private readonly SheepItDatabase sheepItDatabase;

        public ListEnvironmentsHandler(SheepItDatabase sheepItDatabase)
        {
            this.sheepItDatabase = sheepItDatabase;
        }

        public ListEnvironmentsResponse Handle(ListEnvironmentsRequest request)
        {
            var environments = sheepItDatabase.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .SortBy(environment => environment.Rank)
                .ToEnumerable()
                .Select(Map)
                .ToArray();
            
            return new ListEnvironmentsResponse
            {
                Environments = environments
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
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}
