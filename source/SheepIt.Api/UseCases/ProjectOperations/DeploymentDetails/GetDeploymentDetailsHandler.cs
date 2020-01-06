using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Deployments;

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

    public class GetDeploymentDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetDeploymentDetailsHandler>()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class GetDeploymentDetailsHandler : IHandler<GetDeploymentDetailsRequest, GetDeploymentDetailsResponse>
    {
        private readonly SheepItDbContext _dbContext;

        public GetDeploymentDetailsHandler(
            SheepItDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetDeploymentDetailsResponse> Handle(GetDeploymentDetailsRequest request)
        {
            var deployment = await _dbContext.Deployments
                .Include(deployment1 => deployment1.Environment)
                .Include(deployment1 => deployment1.Package)
                .FindByIdAndProjectId(
                    projectId: request.ProjectId,
                    deploymentId: request.DeploymentId
                );

            return new GetDeploymentDetailsResponse
            {
                Id = deployment.Id,
                Status = deployment.Status.ToString(),
                PackageId = deployment.PackageId,
                PackageDescription = deployment.Package.Description,
                EnvironmentId = deployment.Environment.Id,
                EnvironmentDisplayName = deployment.Environment.DisplayName,
                DeployedAt = deployment.StartedAt,
                
                StepResults = deployment
                    .GetOutputStepsOrEmpty()
                    .Select(result => new GetDeploymentDetailsResponse.CommandOutput
                    {
                        Command = result.Command,
                        Successful = result.Successful,
                        Output = result.Output.ToArray()
                    })
                    .ToArray(),
                
                Variables = deployment.Package
                    .GetVariablesForEnvironment(deployment.Environment.Id)
                    .Select(variableForEnvironment => new GetDeploymentDetailsResponse.VariablesForEnvironmentDto
                    {
                        Name = variableForEnvironment.Name,
                        Value = variableForEnvironment.Value
                    })
                    .ToArray()
            };
        }
    }
}