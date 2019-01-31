using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Domain;

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
        private readonly Projects _projects;
        private readonly ReleasesStorage _releasesStorage;
        private readonly IConfiguration _configuration;

        public UpdateReleaseProcessHandler(
            Projects projects,
            ReleasesStorage releasesStorage,
            IConfiguration configuration)
        {
            _projects = projects;
            _releasesStorage = releasesStorage;
            _configuration = configuration;
        }

        public UpdateReleaseProcessResponse Handle(UpdateReleaseProcessRequest request)
        {
            var project = _projects.Get(request.ProjectId);

            var currentCommitSha = ProcessRepository.GetCurrentCommitSha(project, _configuration);

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