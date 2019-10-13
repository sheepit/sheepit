using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class GetEnvironmentsForUpdateRequest : IRequest<GetEnvironmentsForUpdateResponse>
    {
        public string Id { get; set; }
    }

    public class GetEnvironmentsForUpdateResponse
    {
        public string Id { get; set; }
        public string RepositoryUrl { get; set; }
        
        public EnvironmentDto[] Environments { get; set; }

        public class EnvironmentDto
        {
            public int EnvironmentId { get; set; }
            public string DisplayName { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetEnvironmentsForUpdateController : MediatorController
    {
        [HttpPost]
        [Route("project/environment/get-environments-for-update")]
        public async Task<GetEnvironmentsForUpdateResponse> GetProjectDetails(GetEnvironmentsForUpdateRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetEnvironmentsForUpdateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetEnvironmentsForUpdateHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetEnvironmentsForUpdateHandler : IHandler<GetEnvironmentsForUpdateRequest, GetEnvironmentsForUpdateResponse>
    {
        private readonly SheepItDatabase _database;

        public GetEnvironmentsForUpdateHandler(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<GetEnvironmentsForUpdateResponse> Handle(GetEnvironmentsForUpdateRequest request)
        {
            var project = await _database.Projects
                .FindById(request.Id);
            
            var environments = await _database.Environments
                .Find(filter => filter.FromProject(request.Id))
                .Sort(sort => sort.Ascending(environment => environment.Rank)) // todo: create ByRank extension
                .ToArray();
            
            return new GetEnvironmentsForUpdateResponse
            {
                Id = project.Id,
                RepositoryUrl = project.RepositoryUrl,
                Environments = environments
                    .Select(MapEnvironment)
                    .ToArray()
            };
        }

        private static GetEnvironmentsForUpdateResponse.EnvironmentDto MapEnvironment(Environment environment)
        {
            return new GetEnvironmentsForUpdateResponse.EnvironmentDto
            {
                DisplayName = environment.DisplayName,
                EnvironmentId = environment.Id
            };
        }
    }
}