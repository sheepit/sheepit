using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SheepIt.Api.ScriptFiles
{
    public class ProcessFile
    {
        public string Shell { get; set; }
        public string[] Commands { get; set; }

        public static ProcessFile Open(string processDescriptionFilePath)
        {
            // TODO: [ts] Should we have add here some validation i.e. if the file exists.
            // If it is in correct format etc.
            // File validator with appropriate message to the user would be welcome.

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