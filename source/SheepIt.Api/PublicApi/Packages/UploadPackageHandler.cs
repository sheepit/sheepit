using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.Model.DeploymentProcesses;
using SheepIt.Api.Model.Components;

namespace SheepIt.Api.PublicApi.Packages
{
    [Route("api")]
    [ApiController]
    [AllowAnonymous]
    public class UploadPackageController
    {
        private readonly UploadPackageHandler _handler;

        public UploadPackageController(UploadPackageHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("projects/{projectId}/components/{componentId}/scripts")]
        public async Task<int> UploadPackage(
            [FromRoute] string projectId,
            [FromRoute] int componentId,
            IFormFile zipFile)
        {
            return await _handler.Handle(projectId, componentId, zipFile);
        }
    }

    public class UploadPackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UploadPackageHandler>();
        }
    }

    public class UploadPackageHandler
    {
        private readonly SheepItDbContext _dbContext;
        private readonly DeploymentProcessFactory _deploymentProcessFactory;

        public UploadPackageHandler(
            SheepItDbContext dbContext,
            DeploymentProcessFactory deploymentProcessFactory)
        {
            _dbContext = dbContext;
            _deploymentProcessFactory = deploymentProcessFactory;
        }

        public async Task<int> Handle(string projectId, int componentId, IFormFile zipFile)
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

            if (zipFile == null)
            {
                throw new InvalidOperationException(
                    $"Uploaded zip file is empty."
                );
            }
            
            var deploymentProcess = await _deploymentProcessFactory.Create(
                componentId: componentId,
                zipFileBytes: await zipFile.ToByteArray()
            );
            
            _dbContext.DeploymentProcesses.Add(deploymentProcess);

            await _dbContext.SaveChangesAsync();
            
            return deploymentProcess.Id;
        }
    }
}