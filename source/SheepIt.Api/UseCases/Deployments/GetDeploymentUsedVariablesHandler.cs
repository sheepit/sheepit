using System.Linq;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Deployments
{
    public class GetDeploymentUsedVariablesRequest
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
    public class GetDeploymentUsedVariablesController : ControllerBase
    {
        private readonly GetDeploymentUsedVariablesHandler _handler;

        public GetDeploymentUsedVariablesController(GetDeploymentUsedVariablesHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("get-deployment-used-variables")]
        public object ShowDashboard(GetDeploymentUsedVariablesRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class GetDeploymentUsedVariablesHandler
    {
        private readonly Domain.Deployments _deployments;
        private readonly Projects _projects;
        private readonly ReleasesStorage _releasesStorage;

        public GetDeploymentUsedVariablesHandler(Domain.Deployments deployments, Projects projects, ReleasesStorage releasesStorage)
        {
            _deployments = deployments;
            _projects = projects;
            _releasesStorage = releasesStorage;
        }

        public GetDeploymentUsedVariablesResponse Handle(GetDeploymentUsedVariablesRequest request)
        {
            var project = _projects.Get(
                projectId: request.ProjectId
            );

            var deployment = _deployments.Get(
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
