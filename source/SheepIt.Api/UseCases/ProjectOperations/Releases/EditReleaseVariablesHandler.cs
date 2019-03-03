using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectOperations.Releases
{
    public class EditReleaseVariablesRequest : IRequest, IProjectRequest
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
        [Route("project/release/edit-release-variables")]
        public async Task EditReleaseVariables(EditReleaseVariablesRequest request)
        {
            await Handle(request);
        }
    }

    public class EditReleaseVariablesHandler : ISyncHandler<EditReleaseVariablesRequest>
    {
        private readonly ReleasesStorage _releasesStorage;

        public EditReleaseVariablesHandler(ReleasesStorage releasesStorage)
        {
            _releasesStorage = releasesStorage;
        }

        public void Handle(EditReleaseVariablesRequest request)
        {
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

            _releasesStorage.AddSync(newRelease);
        }
    }
    
    public class EditReleaseVariablesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<EditReleaseVariablesHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}