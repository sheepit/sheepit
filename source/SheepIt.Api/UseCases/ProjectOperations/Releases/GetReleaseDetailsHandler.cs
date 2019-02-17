using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Releases
{
    public class GetReleaseDetailsRequest : IRequest<GetReleaseDetailsResponse>
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
    public class GetReleaseDetailsController : MediatorController
    {
        [HttpPost]
        [Route("project/release/get-release-details")]
        public async Task<GetReleaseDetailsResponse> GetReleaseDetails(GetReleaseDetailsRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetReleaseDetailsHandler : ISyncHandler<GetReleaseDetailsRequest, GetReleaseDetailsResponse>
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

    public class GetReleaseDetailsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetReleaseDetailsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}
