using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Infrastructure;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Releases
{
    public class UpdateReleaseProcessRequest
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
    public class UpdateReleaseProcessController : ControllerBase
    {
        [HttpPost]
        [Route("update-release-process")]
        public object UpdateReleaseProcess(UpdateReleaseProcessRequest request)
        {
            var handler = new UpdateReleaseProcessHandler();
            
            return handler.Handle(request);
        }
    }

    public class UpdateReleaseProcessHandler
    {
        public UpdateReleaseProcessResponse Handle(UpdateReleaseProcessRequest request)
        {
            var project = Projects.Get(request.ProjectId);

            var currentCommitSha = ProcessRepository.GetCurrentCommitSha(project);

            var release = ReleasesStorage.GetNewest(request.ProjectId);

            var newRelease = release.WithUpdatedCommitSha(currentCommitSha);

            var releaseId = ReleasesStorage.Add(newRelease);

            return new UpdateReleaseProcessResponse
            {
                CreatedReleaseId = releaseId,
                CreatedFromCommitSha = currentCommitSha
            };
        }
    }
}