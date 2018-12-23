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
    }

    [Route("api")]
    [ApiController]
    public class CreateProjectController : ControllerBase
    {
        [HttpPost]
        [Route("create-project")]
        public void CreateProject(CreateProjectRequest request)
        {
            CreateProjectHandler.Handle(request);
        }
    }

    public static class CreateProjectHandler
    {
        public static void Handle(CreateProjectRequest request)
        {
            var project = new Project
            {
                Id = request.ProjectId,
                RepositoryUrl = request.RepositoryUrl
            };

            Projects.Add(project);

            // first release is created so other operations can copy it
            CreateFirstRelease(project);
        }

        private static void CreateFirstRelease(Project project)
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