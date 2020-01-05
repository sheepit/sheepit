using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
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
        public string PackageDescription { get; set; }
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
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;
        private readonly SheepItDbContext _dbContext;

        public GetDeploymentDetailsHandler(
            GetEnvironmentsQuery getEnvironmentsQuery,
            SheepItDbContext dbContext)
        {
            _getEnvironmentsQuery = getEnvironmentsQuery;
            _dbContext = dbContext;
        }

        public async Task<GetDeploymentDetailsResponse> Handle(GetDeploymentDetailsRequest request)
        {
            var deployment = await _dbContext.Deployments.FindByIdAndProjectId(
                projectId: request.ProjectId,
                deploymentId: request.DeploymentId
            );

            var environment = await _getEnvironmentsQuery
                .GetByProjectAndId(request.ProjectId, deployment.EnvironmentId);

            var package = await _dbContext.Packages.FindByIdAndProjectId(
                packageId: deployment.PackageId,
                projectId: request.ProjectId
            );

            var variablesForEnvironment = package.GetVariablesForEnvironment(environment.Id);
            
            return new GetDeploymentDetailsResponse
            {
                Id = deployment.Id,
                Status = deployment.Status.ToString(),
                PackageId = deployment.PackageId,
                PackageDescription = package.Description,
                EnvironmentId = environment.Id,
                EnvironmentDisplayName = environment.DisplayName,
                DeployedAt = deployment.StartedAt,
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