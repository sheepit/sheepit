using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Releases
{
    public class GetLastReleaseRequest
    {
        public string ProjectId { get; set; }
    }

    public class GetLastReleaseResponse
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
    public class GetLastReleaseController : ControllerBase
    {
        private readonly GetLastReleaseHandler _handler;

        public GetLastReleaseController(GetLastReleaseHandler handler)
        {
            _handler = handler;
        }

        // currently used for editing variables
        [HttpPost]
        [Route("get-last-release")]
        public object GetLastRelease(GetLastReleaseRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class GetLastReleaseHandler
    {
        private readonly ReleasesStorage _releasesStorage;

        public GetLastReleaseHandler(ReleasesStorage releasesStorage)
        {
            _releasesStorage = releasesStorage;
        }

        public GetLastReleaseResponse Handle(GetLastReleaseRequest request)
        {
            var release = _releasesStorage.GetNewest(request.ProjectId);

            return new GetLastReleaseResponse
            {
                Id = release.Id,
                ProjectId = release.ProjectId,
                CommitSha = release.CommitSha,
                CreatedAt = release.CreatedAt,
                Variables = release.Variables.Variables
                    .Select(values => new GetLastReleaseResponse.VariableDto
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