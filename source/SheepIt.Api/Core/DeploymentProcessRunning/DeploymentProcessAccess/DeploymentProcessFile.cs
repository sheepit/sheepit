using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// ReSharper disable ClassNeverInstantiated.Global - it is instantiated by yaml deserializer

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class DeploymentProcessFile
    {
        public string[] Commands { get; set; }

        public static DeploymentProcessFile OpenFromFile(string processDescriptionFilePath)
        {
            // TODO: [ts] Should we have add here some validation i.e. if the file exists.
            // If it is in correct format etc.
            // File validator with appropriate message to the user would be welcome.

            using (var fileStream = File.OpenText(processDescriptionFilePath))
            {
                return OpenFomStream(fileStream);
            }
        }

        public static DeploymentProcessFile OpenFomStream(StreamReader fileStream)
        {
            return new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build()
                .Deserialize<DeploymentProcessFile>(fileStream);
        }
    }
}