using System.IO;
using SheepIt.Api.Runner.DeploymentProcessRunning.DeploymentProcessAccess;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    public static class TestWorkingDir
    {
        public static void Clear(IntegrationTestsFixture fixture)
        {
            var deploymentProcessSettings = fixture.Resolve<DeploymentProcessSettings>();
            var workingDir = deploymentProcessSettings.WorkingDir.ToString();
            
            if (Directory.Exists(workingDir))
            {
                Directory.Delete(workingDir, recursive: true);
            }
        }
    }
}