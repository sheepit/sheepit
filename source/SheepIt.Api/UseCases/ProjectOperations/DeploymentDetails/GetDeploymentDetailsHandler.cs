using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails
{
    public class GetDeploymentDetailsRequest : IRequest<GetDeploymentDetailsResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int DeploymentId { get; set; }
    }

    public class GetDeploymentDetailsResponse
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int PackageId { get; set; }
        public int EnvironmentId { get; set; }
        public string EnvironmentDisplayName { get; set; }
        public DateTime DeployedAt { get; set; }
        public CommandOutput[] StepResults { get; set; }
        public VariablesForEnvironmentDto[] Variables { get; set; }

        public class CommandOutput
        {
            public string Command { get; set; }
            public bool Successful { get; set; }
            public string[] Output { get; set; }
        }
        
        public class VariablesForEnvironmentDto
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetDeploymentDetailsController : MediatorController
    {
        [HttpPost]
        [Route("project/deployment/get-deployment-details")]
        public async Task<GetDeploymentDetailsResponse> GetDeploymentDetails(GetDeploymentDetailsRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetDeploymentDetailsHandler : IHandler<GetDeploymentDetailsRequest, GetDeploymentDetailsResponse>
    {
        private readonly SheepItDatabase _database;

        public GetDeploymentDetailsHandler(SheepItDatabase database)
        {
            _database = database;
        }

        public async Task<GetDeploymentDetailsResponse> Handle(GetDeploymentDetailsRequest request)
        {
            var deployment = await _database.Deployments
                .FindByProjectAndId(request.ProjectId, request.DeploymentId);

            var environment = await _database.Environments
                .FindByProjectAndId(request.ProjectId, deployment.EnvironmentId);

            var package = await _database.Packages
                .FindByProjectAndId(request.ProjectId, deployment.PackageId);

            var variablesForEnvironment = package.GetVariablesForEnvironment(environment.Id);
            
            return new GetDeploymentDetailsResponse
            {
                Id = deployment.Id,
                Status = deployment.Status.ToString(),
                PackageId = deployment.PackageId,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.DeployedAt,
                StepResults = GetStepResults(deployment),
                Variables = variablesForEnvironment
                    .Select(MapVariableForEnvironment)
                    .ToArray()
            };
        }

        private GetDeploymentDetailsResponse.CommandOutput[] GetStepResults(Deployment deployment)
        {
            // todo: this is ugly af
            var steps = deployment.ProcessOutput?.Steps ?? Enumerable.Empty<ProcessStepResult>();

            return steps
                .Select(MapStepResult)
                .ToArray();
        }

        private GetDeploymentDetailsResponse.CommandOutput MapStepResult(ProcessStepResult result)
        {
            return new GetDeploymentDetailsResponse.CommandOutput
            {
                Command = result.Command,
                Successful = result.Successful,
                Output = result.Output.ToArray()
            };
        }

        private static GetDeploymentDetailsResponse.VariablesForEnvironmentDto MapVariableForEnvironment(VariableForEnvironment variableForEnvironment)
        {
            return new GetDeploymentDetailsResponse.VariablesForEnvironmentDto
            {
                Name = variableForEnvironment.Name,
                Value = variableForEnvironment.Value
            };
        }
    }
    
    public class GetDeploymentDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDeploymentDetailsHandler>()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}