using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
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

    public class GetProjectDetailsHandler : ISyncHandler<GetProjectDetailsRequest, GetProjectDetailsResponse>
    {
        private readonly ProjectsStorage _projectsStorage;
        private readonly EnvironmentsStorage _environmentsStorage;

        public GetProjectDetailsHandler(
            ProjectsStorage projectsStorage,
            EnvironmentsStorage environmentsStorage)
        {
            _projectsStorage = projectsStorage;
            _environmentsStorage = environmentsStorage;
        }

        public GetProjectDetailsResponse Handle(GetProjectDetailsRequest options)
        {
            var project = _projectsStorage.Get(options.Id);

            var environments = _environmentsStorage.GetAll(options.Id);
            
            return new GetProjectDetailsResponse
            {
                Id = project.Id,
                RepositoryUrl = project.RepositoryUrl,
                Environments = environments
                    .Select(environment => new GetProjectDetailsResponse.EnvironmentDto
                    {
                        DisplayName = environment.DisplayName,
                        EnvironmentId = environment.Id
                    }).ToArray()
            };
        }
    }
    
    public class GetProjectDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetProjectDetailsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}