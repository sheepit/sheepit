using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.Releases
{
    public class EditReleaseVariablesRequest : IRequest
    {
        public string ProjectId { get; set; }
        public VariableDto[] NewVariables { get; set; }
        
        public class VariableDto
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }

    [Route("api")]
    [ApiController]
    public class EditReleaseVariablesController : MediatorController
    {
        [HttpPost]
        [Route("edit-release-variables")]
        public async Task EditReleaseVariables(EditReleaseVariablesRequest request)
        {
            await Handle(request);
        }
    }

    public class EditReleaseVariablesHandler : ISyncHandler<EditReleaseVariablesRequest>
    {
        private readonly ProjectsStorage _projectsStorage;
        private readonly ReleasesStorage _releasesStorage;

        public EditReleaseVariablesHandler(ProjectsStorage projectsStorage, ReleasesStorage releasesStorage)
        {
            _projectsStorage = projectsStorage;
            _releasesStorage = releasesStorage;
        }

        public void Handle(EditReleaseVariablesRequest request)
        {
            var project = _projectsStorage.Get(request.ProjectId);

            var release = _releasesStorage.GetNewest(request.ProjectId);
            
            var variableValues = request.NewVariables
                .Select(update => new VariableValues
                {
                    Name = update.Name,
                    DefaultValue = update.DefaultValue,
                    EnvironmentValues = update.EnvironmentValues
                })
                .ToArray();
            
            var newRelease = release.WithNewVariables(variableValues);

            _releasesStorage.Add(newRelease);
        }
    }
    
    public class EditReleaseVariablesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<EditReleaseVariablesHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .RegisterIn(builder);
        }
    }
}