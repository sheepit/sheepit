using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Domain;

namespace SheepIt.Api.UseCases.Releases
{
    public class UpdateReleaseVariablesRequest
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
    public class UpdateReleaseVariablesController : ControllerBase
    {
        // meant to be used programatically via public API, e. g. when you want to update single variable, like service version 
        [HttpPost]
        [Route("update-release-variables")]
        public object UpdateReleaseVariables(UpdateReleaseVariablesRequest request)
        {
            var handler = new UpdateReleaseVariablesHandler();
            
            return handler.Handle(request);
        }
    }

    public class UpdateReleaseVariablesHandler
    {
        private readonly Projects _projects = new Projects();
        private readonly ReleasesStorage _releasesStorage = new ReleasesStorage();
        
        public UpdateReleaseVariablesResponse Handle(UpdateReleaseVariablesRequest request)
        {
            var project = _projects.Get(request.ProjectId);

            var release = _releasesStorage.GetNewest(request.ProjectId);

            var variableValueses = request.Updates
                .Select(update => new VariableValues
                {
                    Name = update.Name,
                    DefaultValue = update.DefaultValue,
                    EnvironmentValues = update.EnvironmentValues
                })
                .ToArray();

            var newRelease = release.WithUpdatedVariables(variableValueses);

            var newReleaseId = _releasesStorage.Add(newRelease);

            return new UpdateReleaseVariablesResponse
            {
                CreatedReleaseId = newReleaseId
            };
        }
    }
}