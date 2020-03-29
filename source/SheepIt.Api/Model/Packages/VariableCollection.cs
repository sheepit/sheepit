using System.Linq;

// ReSharper disable MemberCanBePrivate.Global - npgsql requires json fields to have public setters

namespace SheepIt.Api.Model.Packages
{
    public class VariableCollection
    {
        public VariableValues[] Variables { get; set; }

        public VariableCollection()
        {
            Variables = new VariableValues[0];
        }

        public VariableForEnvironment[] GetForEnvironment(int environmentId)
        {
            return Variables
                .Select(variable => variable.ForEnvironment(environmentId))
                .ToArray();
        }
    }
}