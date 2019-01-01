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
        // currently used for editing variables
        [HttpPost]
        [Route("get-last-release")]
        public object GetLastRelease(GetLastReleaseRequest request)
        {
            return GetLastReleaseHandler.Handle(request);
        }
    }

    public static class GetLastReleaseHandler
    {
        public static GetLastReleaseResponse Handle(GetLastReleaseRequest request)
        {
            var release = ReleasesStorage.GetNewest(request.ProjectId);

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