using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SheepIt.Api.PublicApi.Packages.CreatePackage
{
    public class CreatePackageRequest
    {
        public string Description { get; set; }
        public UpdateVariable[] VariableUpdates { get; set; }
        public IFormFile ZipFile { get; set; }

        public class UpdateVariable
        {
            public string Name { get; set; }
            public string DefaultValue { get; set; }
            public Dictionary<int, string> EnvironmentValues { get; set; }
        }
    }
}