namespace SheepIt.Domain
{
    public class Variable
    {
        public string Name { get; }
        public string Value { get; }

        public Variable(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}