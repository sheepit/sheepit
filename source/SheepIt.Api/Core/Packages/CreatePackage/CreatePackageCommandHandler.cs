using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.Model.Components;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.Core.Packages.CreatePackage
{
    public class CreatePackageCommand
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

    public class CreatePackageCommandHandler
    {
        private readonly SheepItDbContext _dbContext;
        private readonly PackageFactory _packageFactory;
        private readonly DeploymentProcessFactory _deploymentProcessFactory;

        public CreatePackageCommandHandler(
            SheepItDbContext dbContext,
            PackageFactory packageFactory,
            DeploymentProcessFactory deploymentProcessFactory)
        {
            _dbContext = dbContext;
            _packageFactory = packageFactory;
            _deploymentProcessFactory = deploymentProcessFactory;
        }
        
        public async Task<int> Handle(CreatePackageCommand command)
        {
            var componentExists = await _dbContext.Components
                .FromProject(command.ProjectId)
                .WithId(command.ComponentId)
                .AnyAsync();

            if (!componentExists)
            {
                throw new InvalidOperationException(
                    $"Component {command.ComponentId} in project {command.ProjectId} doesn't exist."
                );
            }

            var deploymentProcess = await _deploymentProcessFactory.Create(
                componentId: command.ComponentId,
                zipFileBytes: await command.ZipFile.ToByteArray()
            );

            _dbContext.DeploymentProcesses.Add(deploymentProcess);

            var newVariables = MapVariableValues(command.VariableUpdates);
            
            var newPackage = await _packageFactory.Create(
                projectId: command.ProjectId,
                deploymentProcessId: deploymentProcess.Id,
                componentId: command.ComponentId,
                description: command.Description,
                variableCollection: new VariableCollection
                {
                    Variables = newVariables
                }
            );

            _dbContext.Packages.Add(newPackage);

            await _dbContext.SaveChangesAsync();

            return newPackage.Id;
        }

        private VariableValues[] MapVariableValues(CreatePackageCommand.UpdateVariable[] variableUpdates)
        {
            var updatesOrNull = variableUpdates
                ?.Select(MapVariableValue)
                .ToArray();
            
            return updatesOrNull ?? new VariableValues[0];
        }

        private VariableValues MapVariableValue(CreatePackageCommand.UpdateVariable variableUpdate)
        {
            return VariableValues.Create(
                name: variableUpdate.Name,
                defaultValue: variableUpdate.DefaultValue,
                environmentValues: variableUpdate.EnvironmentValues ?? new Dictionary<int, string>()
            );
        }
    }
}