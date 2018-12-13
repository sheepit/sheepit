using System.Collections.Generic;
using System.Linq;

namespace SheepIt.Domain
{
    public class VariableCollection
    {
        public VariableValues[] Variables { get; set; } = new VariableValues[0];

        public VariableForEnvironment[] GetForEnvironment(string environment)
        {
            return Variables
                .Select(variable => variable.ForEnvironment(environment))
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
    }

    public class VariableValues
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public Dictionary<string, string> EnvironmentValues { get; set; }

        public VariableForEnvironment ForEnvironment(string environment)
        {
            return new VariableForEnvironment(Name, ValueForEnvironment(environment));
        }

        private string ValueForEnvironment(string environment)
        {
            if (EnvironmentValues != null && EnvironmentValues.TryGetValue(environment, out var environmentSpecificValue))
            {
                return environmentSpecificValue;
            }

            return DefaultValue;
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