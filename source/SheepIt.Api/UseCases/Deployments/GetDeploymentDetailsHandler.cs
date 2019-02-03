using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Deployments
{
    public class GetDeploymentDetailsRequest : IRequest<GetDeploymentDetailsResponse>
    {
        public string ProjectId { get; set; }
        public int DeploymentId { get; set; }
    }

    public class GetDeploymentDetailsResponse
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int ReleaseId { get; set; }
        public int EnvironmentId { get; set; }
        public string EnvironmentDisplayName { get; set; }
        public DateTime DeployedAt { get; set; }
        public CommandOutput[] StepResults { get; set; }

        public class CommandOutput
        {
            public string Command { get; set; }
            public bool Successful { get; set; }
            public string[] Output { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetDeploymentDetailsController : MediatorController
    {
        [HttpPost]
        [Route("get-deployment-details")]
        public async Task<GetDeploymentDetailsResponse> GetDeploymentDetails(GetDeploymentDetailsRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetDeploymentDetailsHandler : ISyncHandler<GetDeploymentDetailsRequest, GetDeploymentDetailsResponse>
    {
        private readonly Domain.Deployments _deployments;
        private readonly Domain.Environments _environments;
        private readonly Projects _projects;
        private readonly ReleasesStorage _releasesStorage;

        public GetDeploymentDetailsHandler(Domain.Deployments deployments, Domain.Environments environments, Projects projects, ReleasesStorage releasesStorage)
        {
            _deployments = deployments;
            _environments = environments;
            _projects = projects;
            _releasesStorage = releasesStorage;
        }

        public GetDeploymentDetailsResponse Handle(GetDeploymentDetailsRequest request)
        {
            var project = _projects.Get(
                projectId: request.ProjectId
            );

            var deployment = _deployments.Get(
                projectId: request.ProjectId,
                deploymentId: request.DeploymentId
            );

            var environment = _environments.Get(
                environmentId: deployment.EnvironmentId);

            var release = _releasesStorage.Get(
                projectId: request.ProjectId,
                releaseId: deployment.ReleaseId
            );

            return new GetDeploymentDetailsResponse
            {
                Id = deployment.Id,
                Status = deployment.Status.ToString(),
                ReleaseId = deployment.ReleaseId,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.DeployedAt,
                StepResults = GetStepResults(deployment)
            };
        }

        private GetDeploymentDetailsResponse.CommandOutput[] GetStepResults(Deployment deployment)
        {
            // todo: this is ugly af
            var steps = deployment.ProcessOutput?.Steps ?? Enumerable.Empty<ProcessStepResult>();

            return steps
                .Select(MapCommandOutput)
                .ToArray();
        }

        private GetDeploymentDetailsResponse.CommandOutput MapCommandOutput(ProcessStepResult result)
        {
            return new GetDeploymentDetailsResponse.CommandOutput
            {
                Command = result.Command,
                Successful = result.Successful,
                Output = result.Output.ToArray()
            };
        }
    }
    
    public class GetDeploymentDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDeploymentDetailsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}