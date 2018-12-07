using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SheepIt.ConsolePrototype
{
    public class ProcessDescription
    {
        public string Script { get; set; }
    }

    public static class ProcessDescriptionFile
    {
        public static ProcessDescription Open()
        {
            using (var fileStream = File.OpenText("process.yaml"))
            {
                return new DeserializerBuilder()
                    .WithNamingConvention(new UnderscoredNamingConvention())
                    .Build()
                    .Deserialize<ProcessDescription>(fileStream);
            }
        }
    }
}