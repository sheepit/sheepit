﻿using System;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Infrastructure;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases
{
    public class CreateProjectRequest
    {
        public string ProjectId { get; set; }
        public string RepositoryUrl { get; set; }
        public string[] EnvironmentNames { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class CreateProjectController : ControllerBase
    {
        [HttpPost]
        [Route("create-project")]
        public void CreateProject(CreateProjectRequest request)
        {
            var handler = new CreateProjectHandler();
            
            handler.Handle(request);
        }
    }

    public class CreateProjectHandler
    {
        private readonly Projects _projects = new Projects();
        private readonly Domain.Environments _environments = new Domain.Environments();
        
        public void Handle(CreateProjectRequest request)
        {
            var project = new Project
            {
                Id = request.ProjectId,
                RepositoryUrl = request.RepositoryUrl
            };

            _projects.Add(project);

            CreateEnvironments(request);

            // first release is created so other operations can copy it
            CreateFirstRelease(project);
        }

        private void CreateEnvironments(CreateProjectRequest request)
        {
            foreach (var environmentName in request.EnvironmentNames)
            {
                var environment = new Domain.Environment(request.ProjectId, environmentName);
                
                _environments.Add(environment);
            }
        }
        
        private void CreateFirstRelease(Project project)
        {
            var currentCommitSha = ProcessRepository.GetCurrentCommitSha(project);

            ReleasesStorage.Add(new Release
            {
                Variables = new VariableCollection(),
                CommitSha = currentCommitSha,
                CreatedAt = DateTime.UtcNow,
                ProjectId = project.Id
            });
        }
    }
}