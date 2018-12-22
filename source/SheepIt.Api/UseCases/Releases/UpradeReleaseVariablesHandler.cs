using System.Collections.Generic;
using System.Linq;
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
            public Dictionary<string, string> EnvironmentValues { get; set; }
        }
    }

    public class UpdateReleaseVariablesResponse
    {
        public int CreatedReleaseId { get; set; }
    }

    public static class UpdateReleaseVariablesHandler
    {
        public static UpdateReleaseVariablesResponse Handle(UpdateReleaseVariablesRequest request)
        {
            var project = Projects.Get(request.ProjectId);

            var release = ReleasesStorage.GetNewest(request.ProjectId);

            var variableValueses = request.Updates
                .Select(update => new VariableValues
                {
                    Name = update.Name,
                    DefaultValue = update.DefaultValue,
                    EnvironmentValues = update.EnvironmentValues
                })
                .ToArray();

            var newReleasee = release.WithUpdatedVariables(variableValueses);

            var newReleaseId = ReleasesStorage.Add(newReleasee);

            return new UpdateReleaseVariablesResponse
            {
                CreatedReleaseId = newReleaseId
            };
        }
    }
}