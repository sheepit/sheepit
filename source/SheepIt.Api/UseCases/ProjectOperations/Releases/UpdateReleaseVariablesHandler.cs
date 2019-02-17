using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Releases
{
    public class UpdateReleaseVariablesRequest : IRequest<UpdateReleaseVariablesResponse>
    {
        public string ProjectId { get; set; }
        public UpdateVariable[] Updates { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    public class UpdateReleaseVariablesResponse
    {
        public int CreatedReleaseId { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class UpdateReleaseVariablesController : MediatorController
    {
        // meant to be used programatically via public API, e. g. when you want to update single variable, like service version
        [HttpPost]
        [Route("project/release/update-release-variables")]
        public async Task<UpdateReleaseVariablesResponse> UpdateReleaseVariables(UpdateReleaseVariablesRequest request)
        {
            return await Handle(request);
        }
    }

    public class UpdateReleaseVariablesHandler : ISyncHandler<UpdateReleaseVariablesRequest, UpdateReleaseVariablesResponse>
    {
        private readonly ProjectsStorage _projectsStorage;
        private readonly ReleasesStorage _releasesStorage;

        public UpdateReleaseVariablesHandler(ProjectsStorage projectsStorage, ReleasesStorage releasesStorage)
        {
            _projectsStorage = projectsStorage;
            _releasesStorage = releasesStorage;
        }

        public UpdateReleaseVariablesResponse Handle(UpdateReleaseVariablesRequest request)
        {
            var project = _projectsStorage.Get(request.ProjectId);

            var release = _releasesStorage.GetNewest(request.ProjectId);

            var variableValues = request.Updates
                .Select(update => new VariableValues
                {
                    Name = update.Name,
                    DefaultValue = update.DefaultValue,
                    EnvironmentValues = update.EnvironmentValues
                })
                .ToArray();

            var newRelease = release.WithUpdatedVariables(variableValues);

            var newReleaseId = _releasesStorage.Add(newRelease);

            return new UpdateReleaseVariablesResponse
            {
                CreatedReleaseId = newReleaseId
            };
        }
    }
    
    public class UpdateReleaseVariablesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateReleaseVariablesHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}