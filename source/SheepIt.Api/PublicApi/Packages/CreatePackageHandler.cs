using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.PublicApi.Packages
{
    public class CreatePackageRequest : IRequest<CreatePackageResponse>
    {
        public string Description { get; set; }
        public UpdateVariable[] VariableUpdates { get; set; }
        public IFormFile ZipFile { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    public class CreatePackageResponse
    {
        public int CreatedPackageId { get; set; }
    }
    
    public class CreatePackageRequestValidator : AbstractValidator<CreatePackageRequest>
    {
        public CreatePackageRequestValidator()
        {
            RuleFor(request => request.Description)
                .NotNull();
            
            RuleFor(request => request.ZipFile)
                .NotNull();
        }
    }

    [Route("api")]
    [ApiController]
    public class PackageController
    {
        private readonly CreatePackageHandler _handler;

        public PackageController(CreatePackageHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("/projects/{projectId}/components/{componentId}/packages")]
        public async Task<CreatePackageResponse> CreatePackage(
            [FromRoute] string projectId,
            [FromRoute] int componentId,
            [FromBody] CreatePackageRequest request)
        {
            return await _handler.Handle(projectId, componentId, request);
        }
    }

    public class CreatePackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreatePackageHandler>();
        }
    }

    public class CreatePackageHandler
    {
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;
        private readonly DeploymentProcessFactory _deploymentProcessFactory;

        public CreatePackageHandler(
            SheepItDbContext dbContext,
            PackageFactory packageFactory,
            DeploymentProcessFactory deploymentProcessFactory)
        {
            _dbContext = dbContext;
            _packageFactory = packageFactory;
            _deploymentProcessFactory = deploymentProcessFactory;
        }

        public async Task<CreatePackageResponse> Handle(string projectId, int componentId, CreatePackageRequest request)
        {
            var componentExists = await _dbContext.Components
                .FromProject(projectId)
                .WithId(componentId)
                .AnyAsync();

            if (!componentExists)
            {
                throw new InvalidOperationException(
                    $"Component {componentId} in project {projectId} doesn't exist."
                );
            }

            var deploymentProcess = await _deploymentProcessFactory.Create(
                componentId: componentId,
                zipFileBytes: await request.ZipFile.ToByteArray()
            );

            _dbContext.DeploymentProcesses.Add(deploymentProcess);

            var newVariables = MapVariableValues(request.VariableUpdates);
            
            var newPackage = await _packageFactory.Create(
                projectId: projectId,
                deploymentProcessId: deploymentProcess.Id,
                componentId: componentId,
                description: request.Description,
                variableCollection: new VariableCollection
                {
                    Variables = newVariables
                }
            );

            _dbContext.Packages.Add(newPackage);

            await _dbContext.SaveChangesAsync();

            return new CreatePackageResponse
            {
                CreatedPackageId = newPackage.Id
            };
        }

        private VariableValues[] MapVariableValues(CreatePackageRequest.UpdateVariable[] variableUpdates)
        {
            var updatesOrNull = variableUpdates
                ?.Select(MapVariableValue)
                .ToArray();
            
            return updatesOrNull ?? new VariableValues[0];
        }

        private VariableValues MapVariableValue(CreatePackageRequest.UpdateVariable variableUpdate)
        {
            return VariableValues.Create(
                name: variableUpdate.Name,
                defaultValue: variableUpdate.DefaultValue,
                environmentValues: variableUpdate.EnvironmentValues ?? new Dictionary<int, string>()
            );
        }
    }
}