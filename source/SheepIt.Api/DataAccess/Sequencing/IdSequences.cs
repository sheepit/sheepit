namespace SheepIt.Api.DataAccess.Sequencing
{
    public class IdSequence
    {
        public static readonly IdSequence Environment = new IdSequence("environment");
        public static readonly IdSequence Package = new IdSequence("package");
        public static readonly IdSequence Deployment = new IdSequence("deployment");

        public string Name { get; }

        private IdSequence(string name)
        {
            Name = name;
        }
    }
}