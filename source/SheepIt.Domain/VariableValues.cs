using System.Collections.Generic;

namespace SheepIt.Domain
{
    public class VariableValues
    {
        public string Default { get; set; }

        public Dictionary<string, string> Environments { get; set; }

        public string ValueForEnvironment(string environment)
        {
            if (Environments != null && Environments.TryGetValue(environment, out var environmentSpecificValue))
            {
                return environmentSpecificValue;
            }

            return Default;
        }
    }
}