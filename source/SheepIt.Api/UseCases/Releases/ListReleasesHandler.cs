using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.Releases
{
    public class ListReleasesRequest : IRequest<ListReleasesResponse>
    {
        public string ProjectId { get; set; }
    }

    public class ListReleasesResponse
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
    public class ListReleasesController : MediatorController
    {
        [HttpPost]
        [Route("list-releases")]
        public async Task<ListReleasesResponse> ListReleases(ListReleasesRequest request)
        {
            return await Handle(request);
        }
    }

    public class ListReleasesHandler : ISyncHandler<ListReleasesRequest, ListReleasesResponse>
    {
        private readonly SheepItDatabase _sheepItDatabase;

        public ListReleasesHandler(SheepItDatabase sheepItDatabase)
        {
            _sheepItDatabase = sheepItDatabase;
        }

        public ListReleasesResponse Handle(ListReleasesRequest options)
        {
            var releases = _sheepItDatabase.Releases
                .Find(filter => filter.FromProject(options.ProjectId))
                .SortBy(release => release.CreatedAt)
                .ToEnumerable()
                .Select(Map)
                .ToArray();

            return new ListReleasesResponse
            {
                Releases = releases
            };
        }

        private ListReleasesResponse.ReleaseDto Map(Release release)
        {
            return new ListReleasesResponse.ReleaseDto
            {
                Id = release.Id,
                CommitSha = release.CommitSha,
                CreatedAt = release.CreatedAt
            };
        }
    }
    
    public class ListReleasesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<ListReleasesHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}