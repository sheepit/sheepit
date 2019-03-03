using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Releases
{
    public class GetLastReleaseRequest : IRequest<GetLastReleaseResponse>, IProjectRequest
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
    public class GetLastReleaseController : MediatorController
    {
        // currently used for editing variables
        [HttpPost]
        [Route("project/release/get-last-release")]
        public async Task<GetLastReleaseResponse> GetLastRelease(GetLastReleaseRequest request)
        {
            return await Handle(request);
        }
    }

    public class GetLastReleaseHandler : ISyncHandler<GetLastReleaseRequest, GetLastReleaseResponse>
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
    
    public class GetLastReleaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<GetLastReleaseHandler>()
                .AsAsyncHandler()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}