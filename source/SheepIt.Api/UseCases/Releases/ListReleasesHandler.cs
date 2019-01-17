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
        private readonly ListReleasesHandler _handler;

        public ListReleasesController(ListReleasesHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("list-releases")]
        public object ListReleases(ListReleasesRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class ListReleasesHandler
    {
        private readonly SheepItDatabase sheepItDatabase = new SheepItDatabase();
        
        public ListReleaseResponse Handle(ListReleasesRequest options)
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

        private ListReleaseResponse.ReleaseDto Map(Release release)
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