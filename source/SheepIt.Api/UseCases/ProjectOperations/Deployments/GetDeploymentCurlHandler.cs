using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs.Model;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.ProjectContext;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Deployments
{
    public class GetDeploymentCurlRequest : IRequest<GetDeploymentCurlResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int PackageId { get; set; }
    }

    public class GetDeploymentCurlResponse
    {
        public string Curl { get; set; }
    }

    [Route("frontendApi")]
    [ApiController]
    public class GetDeploymentCurlController : MediatorController
    {
        [HttpPost]
        [Route("project/deployments/get-curl")]
        public async Task<GetDeploymentCurlResponse> ShowDashboard(GetDeploymentCurlRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetDeploymentCurlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDeploymentCurlHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetDeploymentCurlHandler : IHandler<GetDeploymentCurlRequest, GetDeploymentCurlResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetDeploymentCurlHandler(SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetDeploymentCurlResponse> Handle(GetDeploymentCurlRequest request)
        {
            var package = _dbContext
                .Packages
                .FirstOrDefault(x => x.Id == request.PackageId && x.ProjectId == request.ProjectId);
            
            if (package == null)
            {
                throw new InvalidOperationException(
                    $"Package with id {request.PackageId} in project {request.ProjectId} could not be found."
                );
            }
            
            var mainUrl = $"curl -X POST https://localhost:5001/api/projects/{package.ProjectId}/components/{package.ComponentId}/packages/{package.Id}/deployments -H  \"Content-Type: application/json-patch+json\" -d {{\"environmentId\":__ENVIRONMENT_ID__}}";
            
            return new GetDeploymentCurlResponse
            {
                Curl = mainUrl
            };
        }
    }
}
