using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SheepIt.Api.Infrastructure.Utils;

// ReSharper disable MemberCanBePrivate.Global - npgsql requires json fields to have public setters

namespace SheepIt.Api.Model.Packages
{
    public class VariableValues
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public Dictionary<string, string> ActualEnvironmentValues { get; set; }

        public static VariableValues Create(string name, string defaultValue, Dictionary<int, string> environmentValues)
        {
            return new VariableValues
            {
                Name = name,
                DefaultValue = defaultValue,
                ActualEnvironmentValues = environmentValues.ToDictionary(
                    pair => pair.Key.ToString(CultureInfo.InvariantCulture),
                    pair => pair.Value
                )
            };
        }

        public Dictionary<int, string> GetEnvironmentValues()
        {
            return ActualEnvironmentValues.ToDictionary(
                pair => pair.Key.ToInt(),
                pair => pair.Value
            );
        }

        public VariableForEnvironment ForEnvironment(int environmentId)
        {
            return new VariableForEnvironment(Name, ValueForEnvironment(environmentId));
        }

        private string ValueForEnvironment(int environmentId)
        {
            var valueFound = GetEnvironmentValues()
                .TryGetValue(environmentId, out var environmentSpecificValue);
            
            return valueFound
                ? environmentSpecificValue 
                : DefaultValue;
        }
    }
}