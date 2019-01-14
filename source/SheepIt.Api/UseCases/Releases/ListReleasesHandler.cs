using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
        private static readonly SheepItDatabase sheepItDatabase = new SheepItDatabase();
        
        public static ListReleaseResponse Handle(ListReleasesRequest options)
        {
            var releases = sheepItDatabase.Releases
                .Find(filter => filter.FromProject(options.ProjectId))
                .SortBy(release => release.CreatedAt)
                .ToEnumerable()
                .Select(Map)
                .ToArray();

            return new ListReleaseResponse
            {
                Releases = releases
            };
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