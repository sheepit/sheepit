using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;

namespace SheepIt.Api.UseCases.Deployments
{
    public class GetDeploymentUsedVariablesRequest : IRequest<GetDeploymentUsedVariablesResponse>
    {
        public string ProjectId { get; set; }
        public int DeploymentId { get; set; }
        public int EnvironmentId { get; set; }
    }

    public class GetDeploymentUsedVariablesResponse
    {
        public VariablesForEnvironmentDto[] Values { get; set; }
    }

    public class VariablesForEnvironmentDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class GetDeploymentUsedVariablesController : MediatorController
    {
        [HttpPost]
        [Route("get-deployment-used-variables")]
        public async Task<GetDeploymentUsedVariablesResponse> ShowDashboard(GetDeploymentUsedVariablesRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetDeploymentUsedVariablesHandler : ISyncHandler<GetDeploymentUsedVariablesRequest, GetDeploymentUsedVariablesResponse>
    {
        private readonly Core.Deployments.DeploymentsStorage _deploymentsStorage;
        private readonly ProjectsStorage _projectsStorage;
        private readonly ReleasesStorage _releasesStorage;

        public GetDeploymentUsedVariablesHandler(Core.Deployments.DeploymentsStorage deploymentsStorage, ProjectsStorage projectsStorage, ReleasesStorage releasesStorage)
        {
            _deploymentsStorage = deploymentsStorage;
            _projectsStorage = projectsStorage;
            _releasesStorage = releasesStorage;
        }

        public GetDeploymentUsedVariablesResponse Handle(GetDeploymentUsedVariablesRequest request)
        {
            var project = _projectsStorage.Get(
                projectId: request.ProjectId
            );

            var deployment = _deploymentsStorage.Get(
                projectId: request.ProjectId,
                deploymentId: request.DeploymentId
            );

            var release = _releasesStorage.Get(
                projectId: request.ProjectId,
                releaseId: deployment.ReleaseId
            );

            var envVariables = release.GetVariablesForEnvironment(request.EnvironmentId);

            var response = new GetDeploymentUsedVariablesResponse
            {
                Values = ConvertToDto(envVariables)
            };

            return response;
        }

        private VariablesForEnvironmentDto[] ConvertToDto(VariableForEnvironment[] values)
        {
            return values.Select(x => new VariablesForEnvironmentDto
            {
                Name = x.Name,
                Value = x.Value
            }).ToArray();
        }
    }
}
