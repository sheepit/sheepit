using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Releases
{
    public class GetReleaseDetailsRequest
    {
        public string ProjectId { get; set; }
        public int ReleaseId { get; set; }
    }

    public class GetReleaseDetailsResponse
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string CommitSha { get; set; }
        public DateTime CreatedAt { get; set; }
        public VariableDto[] Variables { get; set; }

        public class VariableDto
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class GetReleaseDetailsController : ControllerBase
    {
        private readonly GetReleaseDetailsHandler _handler;

        public GetReleaseDetailsController(GetReleaseDetailsHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("get-release-details")]
        public object GetReleaseDetails(GetReleaseDetailsRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class GetReleaseDetailsHandler
    {
        private readonly ReleasesStorage _releasesStorage;

        public GetReleaseDetailsHandler(ReleasesStorage releasesStorage)
        {
            _releasesStorage = releasesStorage;
        }

        public GetReleaseDetailsResponse Handle(GetReleaseDetailsRequest request)
        {
            var release = _releasesStorage.Get(
                projectId: request.ProjectId,
                releaseId: request.ReleaseId
            );

            return new GetReleaseDetailsResponse
            {
                Id = release.Id,
                ProjectId = release.ProjectId,
                CommitSha = release.CommitSha,
                CreatedAt = release.CreatedAt,
                Variables = release.Variables.Variables
                    .Select(values => new GetReleaseDetailsResponse.VariableDto
                    {
                        Name = values.Name,
                        DefaultValue = values.DefaultValue,
                        EnvironmentValues = values.EnvironmentValues.ToDictionary(pair => pair.Key, pair => pair.Value)
                    })
                    .ToArray()
            };
        }
    }
}
