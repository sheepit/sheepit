namespace SheepIt.Api.Model.Packages
{
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