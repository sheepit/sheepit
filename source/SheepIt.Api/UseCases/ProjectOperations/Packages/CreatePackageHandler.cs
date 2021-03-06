﻿﻿using System;
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
using SheepIt.Api.Infrastructure.ProjectContext;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.UseCases.ProjectOperations.Packages
{
    public class CreatePackageRequest : IRequest<CreatePackageResponse>, IProjectRequest
    {
        public string ProjectId { get; set; }
        public int ComponentId { get; set; }
        
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

    public class CreatePackageRequestValidator : AbstractValidator<CreatePackageRequest>
    {
        public CreatePackageRequestValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull();
            
            RuleFor(request => request.Description)
                .NotNull();
        }
    }

    public class CreatePackageResponse
    {
        public int CreatedPackageId { get; set; }
    }

    [Route("frontendApi")]
    [ApiController]
    public class CreatePackageController : MediatorController
    {
        [HttpPost]
        [Route("project/package/create-package")]
        public async Task<CreatePackageResponse> CreatePackage(
            [FromForm] CreatePackageRequest request)
        {
            return await Handle(request);
        }
    }

    public class CreatePackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<CreatePackageHandler>()
                .InProjectLock()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class CreatePackageHandler : IHandler<CreatePackageRequest, CreatePackageResponse>
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

        public async Task<CreatePackageResponse> Handle(CreatePackageRequest request)
        {
            var componentExists = await _dbContext.Components
                .FromProject(request.ProjectId)
                .WithId(request.ComponentId)
                .AnyAsync();

            if (!componentExists)
            {
                throw new InvalidOperationException(
                    $"Component {request.ComponentId} in project {request.ProjectId} doesn't exist."
                );
            }

            var deploymentProcessId = await GetOrCreateDeploymentProcess(request);

            var newVariables = MapVariableValues(request.VariableUpdates);
            
            var newPackage = await _packageFactory.Create(
                projectId: request.ProjectId,
                deploymentProcessId: deploymentProcessId,
                componentId: request.ComponentId,
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

        private async Task<int> GetOrCreateDeploymentProcess(CreatePackageRequest request)
        {
            if (request.ZipFile == null)
            {
                return await GetLastDeploymentProcess(request);
            }
            
            var deploymentProcess = await _deploymentProcessFactory.Create(
                componentId: request.ComponentId,
                zipFileBytes: await request.ZipFile.ToByteArray()
            );
            
            _dbContext.DeploymentProcesses.Add(deploymentProcess);

            return deploymentProcess.Id;
        }

        private async Task<int> GetLastDeploymentProcess(CreatePackageRequest request)
        {
            return await _dbContext.Packages
                .FromProject(request.ProjectId)
                .FromComponent(request.ComponentId)
                .OrderByNewest()
                .Select(lastPackage => lastPackage.DeploymentProcessId)
                .FirstAsync();
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