using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Releases
{
    public class ListReleasesRequest
    {
        public string ProjectId { get; set; }
    }

    public class ListReleaseResponse
    {
        public ReleaseDto[] Releases { get; set; }

        public class ReleaseDto
        {
            public int Id { get; set; }
            public string CommitSha { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class ListReleasesController : ControllerBase
    {
        [HttpPost]
        [Route("list-releases")]
        public object ListReleases(ListReleasesRequest request)
        {
            return ListReleasesHandler.Handle(request);
        }
    }

    public static class ListReleasesHandler
    {
        public static ListReleaseResponse Handle(ListReleasesRequest options)
        {
            using (var database = Database.Open())
            {
                var releaseCollection = database.GetCollection<Release>();

                var releases = releaseCollection
                    .Find(release => release.ProjectId == options.ProjectId)
                    .OrderBy(release => release.CreatedAt)
                    .Select(Map)
                    .ToArray();

                return new ListReleaseResponse
                {
                    Releases = releases
                };
            }
        }

        private static ListReleaseResponse.ReleaseDto Map(Release release)
        {
            return new ListReleaseResponse.ReleaseDto
            {
                Id = release.Id,
                CommitSha = release.CommitSha,
                CreatedAt = release.CreatedAt
            };
        }
    }
}