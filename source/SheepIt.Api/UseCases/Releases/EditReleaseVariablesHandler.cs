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
        [HttpPost]
        [Route("edit-release-variables")]
        public object EditReleaseVariables(EditReleaseVariablesRequest request)
        {
            var handler = new EditReleaseVariablesHandler();
            
            return handler.Handle(request);
        }
    }

    public class EditReleaseVariablesHandler
    {
        private readonly Projects _projects = new Projects();
        
        public EditReleaseVariablesResponse Handle(EditReleaseVariablesRequest request)
        {
            var project = _projects.Get(request.ProjectId);

            var release = ReleasesStorage.GetNewest(request.ProjectId);
            
            var variableValues = request.NewVariables
                .Select(update => new VariableValues
                {
                    Name = update.Name,
                    DefaultValue = update.DefaultValue,
                    EnvironmentValues = update.EnvironmentValues
                })
                .ToArray();
            
            var newRelease = release.WithNewVariables(variableValues);

            ReleasesStorage.Add(newRelease);
            
            return new EditReleaseVariablesResponse();
        }
    }
}