using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Releases
{
    public class EditReleaseVariablesRequest
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

    public class EditReleaseVariablesResponse
    {
    }

    [Route("api")]
    [ApiController]
    public class EditReleaseVariablesController : ControllerBase
    {
        private readonly EditReleaseVariablesHandler _handler;

        public EditReleaseVariablesController(EditReleaseVariablesHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        [Route("edit-release-variables")]
        public object EditReleaseVariables(EditReleaseVariablesRequest request)
        {
            return _handler.Handle(request);
        }
    }

    public class EditReleaseVariablesHandler
    {
        private readonly Projects _projects;
        private readonly ReleasesStorage _releasesStorage;

        public EditReleaseVariablesHandler(Projects projects, ReleasesStorage releasesStorage)
        {
            _projects = projects;
            _releasesStorage = releasesStorage;
        }

        public EditReleaseVariablesResponse Handle(EditReleaseVariablesRequest request)
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
            
            return new EditReleaseVariablesResponse();
        }
    }
}