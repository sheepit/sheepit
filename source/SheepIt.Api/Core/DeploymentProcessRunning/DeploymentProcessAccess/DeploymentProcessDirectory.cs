using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class DeploymentProcessDirectory
    {
        public const string ProcessFileName = "process.yaml";
        
        public LocalPath Path { get; }
        
        public DeploymentProcessDirectory(LocalPath path)
        {
            Path = path;
        }

        public DeploymentProcessFile OpenProcessDescriptionFile()
        {
            var processDescriptionFilePath = Path.AddSegment(ProcessFileName).ToString();

            return DeploymentProcessFile.OpenFromFile(processDescriptionFilePath);
        }
    }
}