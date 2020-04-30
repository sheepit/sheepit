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
        public DateTime DeployedAt { get; set; }
        
        public int PackageId { get; set; }
        public string PackageDescription { get; set; }
        
        public int EnvironmentId { get; set; }
        public string EnvironmentDisplayName { get; set; }
        
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        
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

    [Route("frontendApi")]
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
            var foundDeployment = await _dbContext.Deployments
                .Include(deployment => deployment.Environment)
                .Include(deployment => deployment.Package)
                .Include(deployment => deployment.Package.Component)
                .FindByIdAndProjectId(
                    projectId: request.ProjectId,
                    deploymentId: request.DeploymentId
                );

            return new GetDeploymentDetailsResponse
            {
                Id = foundDeployment.Id,
                Status = foundDeployment.Status.ToString(),
                DeployedAt = foundDeployment.StartedAt,
                
                EnvironmentId = foundDeployment.Environment.Id,
                EnvironmentDisplayName = foundDeployment.Environment.DisplayName,
                
                ComponentId = foundDeployment.Package.ComponentId,
                ComponentName = foundDeployment.Package.Component.Name,
                
                PackageId = foundDeployment.PackageId,
                PackageDescription = foundDeployment.Package.Description,
                
                StepResults = foundDeployment
                    .GetOutputStepsOrEmpty()
                    .Select(result => new GetDeploymentDetailsResponse.CommandOutput
                    {
                        Command = result.Command,
                        Successful = result.Successful,
                        Output = result.Output.ToArray()
                    })
                    .ToArray(),
                
                Variables = foundDeployment.Package
                    .GetVariablesForEnvironment(foundDeployment.Environment.Id)
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