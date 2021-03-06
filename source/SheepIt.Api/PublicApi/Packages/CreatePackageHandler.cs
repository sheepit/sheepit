﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.PublicApi.Packages
{
    public class CreatePackageRequest : IRequest<CreatePackageResponse>
    {
        public int DeploymentProcessId { get; set; }
        public string Description { get; set; }
        public UpdateVariable[] VariableUpdates { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<string, string> EnvironmentValues { get; set; }
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
        }
    }

    [Route("api")]
    [ApiController]
    [AllowAnonymous]
    public class CreatePackageController
    {
        private readonly CreatePackageHandler _handler;

        public CreatePackageController(CreatePackageHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("projects/{projectId}/components/{componentId}/packages")]
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

        public CreatePackageHandler(
            SheepItDbContext dbContext,
            PackageFactory packageFactory)
        {
            _dbContext = dbContext;
            _packageFactory = packageFactory;
        }

        public async Task<CreatePackageResponse> Handle(string projectId, int componentId, CreatePackageRequest request)
        {
            var componentExists = await _dbContext
                .Components
                .FromProject(projectId)
                .WithId(componentId)
                .AnyAsync();

            if (!componentExists)
            {
                throw new InvalidOperationException(
                    $"Component {componentId} in project {projectId} doesn't exist."
                );
            }

            var deploymentProcessId = await GetDeploymentProcess(componentId, request.DeploymentProcessId);

            var lastPackage = await GetLastPackage(projectId, componentId);

            var mappedVariables = MapVariables(request.VariableUpdates);
            
            var updatedVariables = UpdateExistingValues(lastPackage.Variables.Variables, mappedVariables);
            
            var newPackage = await _packageFactory.Create(
                projectId: projectId,
                deploymentProcessId: deploymentProcessId,
                componentId: componentId,
                description: request.Description,
                variableCollection: new VariableCollection
                {
                    Variables = updatedVariables
                }
            );

            _dbContext.Packages.Add(newPackage);

            await _dbContext.SaveChangesAsync();

            return new CreatePackageResponse
            {
                CreatedPackageId = newPackage.Id
            };
        }

        private async Task<int> GetDeploymentProcess(int componentId, int deploymentProcessId)
        {
            return await _dbContext.DeploymentProcesses
                .Where(x => x.ComponentId == componentId)
                .Where(x => x.Id == deploymentProcessId)
                .Select(x => x.Id)
                .FirstAsync();
        }

        private async Task<Package> GetLastPackage(string projectId, int componentId)
        {
            return await _dbContext.Packages
                .FromProject(projectId)
                .FromComponent(componentId)
                .OrderByNewest()
                .FirstAsync();
        }

        internal static VariableValues[] UpdateExistingValues(
            VariableValues[] lastPackageVariables, VariableValues[] mappedRequest)
        {
            return lastPackageVariables
                .Concat(mappedRequest)
                .ToLookup(ks => ks.Name)
                .Select(g => g.Aggregate((item1, item2) => new VariableValues
                {
                    Name = item2.Name,
                    DefaultValue = item2.DefaultValue,
                    ActualEnvironmentValues = item1
                        .ActualEnvironmentValues
                        .Concat(item2.ActualEnvironmentValues)
                        .ToLookup(envValue => envValue.Key)
                        .Select(g2 => g2.Aggregate(
                            (_, value2) =>
                            new KeyValuePair<string, string>(value2.Key, value2.Value)))
                        .ToDictionary(pair => pair.Key, pair => pair.Value)
                }))
                .ToArray();
        }

        internal static VariableValues[] MapVariables(CreatePackageRequest.UpdateVariable[] requestVariableUpdates)
        {
            return requestVariableUpdates
                .Select(x => new VariableValues
                {
                    Name = x.Name,
                    DefaultValue = x.DefaultValue,
                    ActualEnvironmentValues = x.EnvironmentValues
                })
                .ToArray();
        }
    }
}