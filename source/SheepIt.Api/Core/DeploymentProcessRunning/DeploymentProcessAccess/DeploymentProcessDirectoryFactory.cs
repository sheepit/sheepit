using System.IO;
using System.IO.Compression;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess
{
    public class DeploymentProcessDirectoryFactory
    {
        public DeploymentProcessDirectory CreateFromZip(byte[] deploymentProcessZip, LocalPath toDirectory)
        {
            var deploymentProcessZipPath = toDirectory.AddSegment("process.zip");
            var processPath = toDirectory.AddSegment("process");

            Directory.CreateDirectory(toDirectory.ToString());
            
            File.WriteAllBytes(
                path: deploymentProcessZipPath.ToString(),
                bytes: deploymentProcessZip
            );

            ZipFile.ExtractToDirectory(
                sourceArchiveFileName: deploymentProcessZipPath.ToString(), 
                destinationDirectoryName: processPath.ToString()
            );
            
            return new DeploymentProcessDirectory(processPath);
        }
    }
}