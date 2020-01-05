using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.Core.Packages
{
    public class VariableCollection
    {
        public VariableValues[] Variables { get; set; } = new VariableValues[0];

        public VariableForEnvironment[] GetForEnvironment(int environmentId)
        {
            return Variables
                .Select(variable => variable.ForEnvironment(environmentId))
                .ToArray();
        }

        public VariableCollection WithUpdatedVariables(VariableValues[] newVariables)
        {
            var dict = Variables.ToDictionary(
                variable => variable.Name,
                variable => variable
            );

            foreach (var newVariable in newVariables)
            {
                dict[newVariable.Name] = newVariable;
            }

            var updatedVariables = dict
                .Select(pair => pair.Value)
                .ToArray();

            return new VariableCollection
            {
                Variables = updatedVariables
            };
        }
    }

    public class VariableValues
    {
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
        
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        
        // todo: [rt] postgres probably requires objects to have string keys, but it can be checked
        public Dictionary<string, string> ActualEnvironmentValues { get; set; }

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

    public class VariableForEnvironment
    {
        public string Name { get; }
        public string Value { get; }

        public VariableForEnvironment(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}