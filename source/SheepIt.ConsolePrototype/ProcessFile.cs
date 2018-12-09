using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SheepIt.ConsolePrototype
{
    public class ProcessFile
    {
        public string[] Commands { get; set; }

        public static ProcessFile Open(string processDescriptionFilePath)
        {
            using (var fileStream = File.OpenText(processDescriptionFilePath))
            {
                return new DeserializerBuilder()
                    .WithNamingConvention(new UnderscoredNamingConvention())
                    .Build()
                    .Deserialize<ProcessFile>(fileStream);
            }
        }
    }
}