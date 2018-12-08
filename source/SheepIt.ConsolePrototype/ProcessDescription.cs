using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SheepIt.ConsolePrototype
{
    // todo: join these

    public class ProcessDescription
    {
        public string Script { get; set; }
    }

    public static class ProcessDescriptionFile
    {
        public static ProcessDescription Open(string processDescriptionFilePath)
        {
            using (var fileStream = File.OpenText(processDescriptionFilePath))
            {
                return new DeserializerBuilder()
                    .WithNamingConvention(new UnderscoredNamingConvention())
                    .Build()
                    .Deserialize<ProcessDescription>(fileStream);
            }
        }
    }
}