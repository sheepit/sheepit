using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class DeploymentProcessDirectory
    {
        public LocalPath Path { get; }
        
        public DeploymentProcessDirectory(LocalPath path)
        {
            Path = path;
        }

        public DeploymentProcessFile OpenProcessDescriptionFile()
        {
            var processDescriptionFilePath = Path.AddSegment("process.yaml").ToString();

            return DeploymentProcessFile.Open(processDescriptionFilePath);
        }
    }
}