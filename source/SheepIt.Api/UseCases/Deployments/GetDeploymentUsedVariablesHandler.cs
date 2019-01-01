using System.Linq;
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
        [HttpPost]
        [Route("get-deployment-used-variables")]
        public object ShowDashboard(GetDeploymentUsedVariablesRequest request)
        {
            return GetDeploymentUsedVariablesHandler.Handle(request);
        }
    }

    public static class GetDeploymentUsedVariablesHandler
    {
        public static GetDeploymentUsedVariablesResponse Handle(GetDeploymentUsedVariablesRequest request)
        {
            var project = Projects.Get(
                projectId: request.ProjectId
            );

            var deployment = Domain.Deployments.Get(
                projectId: request.ProjectId,
                deploymentId: request.DeploymentId
            );

            var release = ReleasesStorage.Get(
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

        public static VariablesForEnvironmentDto[] ConvertToDto(VariableForEnvironment[] values)
        {
            return values.Select(x => new VariablesForEnvironmentDto
            {
                Name = x.Name,
                Value = x.Value
            }).ToArray();
        }
    }
}
