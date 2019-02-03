﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Handlers;

namespace SheepIt.Api.UseCases.Releases
{
    public class UpdateReleaseProcessRequest : IRequest<UpdateReleaseProcessResponse>
    {
        public string ProjectId { get; set; }
    }

    public class UpdateReleaseProcessResponse
    {
        public int CreatedReleaseId { get; set; }
        public string CreatedFromCommitSha { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateReleaseProcessController : MediatorController
    {
        [HttpPost]
        [Route("update-release-process")]
        public async Task<UpdateReleaseProcessResponse> UpdateReleaseProcess(UpdateReleaseProcessRequest request)
        {
            return await Handle(request);
        }
    }

    public class UpdateReleaseProcessHandler : ISyncHandler<UpdateReleaseProcessRequest, UpdateReleaseProcessResponse>
    {
        private readonly ProjectsStorage _projectsStorage;
        private readonly ReleasesStorage _releasesStorage;
        private readonly ProcessRepositoryFactory _processRepositoryFactory;

        public UpdateReleaseProcessHandler(ProjectsStorage projectsStorage, ReleasesStorage releasesStorage, ProcessRepositoryFactory processRepositoryFactory)
        {
            _projectsStorage = projectsStorage;
            _releasesStorage = releasesStorage;
            _processRepositoryFactory = processRepositoryFactory;
        }

        public UpdateReleaseProcessResponse Handle(UpdateReleaseProcessRequest request)
        {
            var project = _projectsStorage.Get(request.ProjectId);

            var currentCommitSha = _processRepositoryFactory.GetCurrentCommitSha(project);

            var release = _releasesStorage.GetNewest(request.ProjectId);

            var newRelease = release.WithUpdatedCommitSha(currentCommitSha);

            var releaseId = _releasesStorage.Add(newRelease);

            return new UpdateReleaseProcessResponse
            {
                CreatedReleaseId = releaseId,
                CreatedFromCommitSha = currentCommitSha
            };
        }
    }
}