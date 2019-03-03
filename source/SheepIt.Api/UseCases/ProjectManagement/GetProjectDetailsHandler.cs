using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class GetProjectDetailsRequest : IRequest<GetProjectDetailsResponse>
    {
        public string Id { get; set; }
    }

    public class GetProjectDetailsResponse
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
    public class GetProjectDetailsController : MediatorController
    {
        [HttpPost]
        [Route("get-project-details")]
        public async Task<GetProjectDetailsResponse> GetProjectDetails(GetProjectDetailsRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetProjectDetailsHandler : IHandler<GetProjectDetailsRequest, GetProjectDetailsResponse>
    {
        private readonly SheepItDatabase _database;

        public GetProjectDetailsHandler(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<GetProjectDetailsResponse> Handle(GetProjectDetailsRequest request)
        {
            var project = await _database.Projects
                .FindById(request.Id);
            
            var environments = await _database.Environments
                .Find(filter => filter.FromProject(request.Id))
                .Sort(sort => sort.Ascending(environment => environment.Rank)) // todo: create ByRank extension
                .ToArray();
            
            return new GetProjectDetailsResponse
            {
                Id = project.Id,
                RepositoryUrl = project.RepositoryUrl,
                Environments = environments
                    .Select(MapEnvironment)
                    .ToArray()
            };
        }

        private static GetProjectDetailsResponse.EnvironmentDto MapEnvironment(Environment environment)
        {
            return new GetProjectDetailsResponse.EnvironmentDto
            {
                DisplayName = environment.DisplayName,
                EnvironmentId = environment.Id
            };
        }
    }
    
    public class GetProjectDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetProjectDetailsHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }
}