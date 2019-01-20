using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Domain;

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
        private readonly Projects _projects;
        private readonly ReleasesStorage _releasesStorage;

        public EditReleaseVariablesHandler(Projects projects, ReleasesStorage releasesStorage)
        {
            _projects = projects;
            _releasesStorage = releasesStorage;
        }

        public void Handle(EditReleaseVariablesRequest request)
        {
            var project = _projects.Get(request.ProjectId);

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
}