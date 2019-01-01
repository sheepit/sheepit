using System.Collections.Generic;
using System.Linq;

namespace SheepIt.Domain
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
            var dict = Variables.ToDictionary(keySelector: variable => variable.Name, elementSelector: variable => variable);

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

        // todo: clone won't be necessary, when databse document is separated from immutable domain object
        public VariableCollection Clone()
        {
            return new VariableCollection
            {
                Variables = Variables
                    .Select(variable => variable.Clone())
                    .ToArray()
            };
        }
    }

    public class VariableValues
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public Dictionary<int, string> EnvironmentValues { get; set; }

        public VariableForEnvironment ForEnvironment(int environmentId)
        {
            return new VariableForEnvironment(Name, ValueForEnvironment(environmentId));
        }

        private string ValueForEnvironment(int environmentId)
        {
            if (EnvironmentValues != null && EnvironmentValues.TryGetValue(environmentId, out var environmentSpecificValue))
            {
                return environmentSpecificValue;
            }

            return DefaultValue;
        }

        public VariableValues Clone()
        {
            return new VariableValues
            {
                Name = Name,
                DefaultValue = DefaultValue,
                EnvironmentValues = EnvironmentValues.ToDictionary(
                    keySelector: pair => pair.Key,
                    elementSelector: pair => pair.Value
                )
            };
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