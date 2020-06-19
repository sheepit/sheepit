using Microsoft.AspNetCore.Http;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;

namespace SheepIt.Api.Tests.IntegrationTests.TestProcess
{
    public static class TestProcessZipArchives
    {
        public static FormFile TestProcess => FormFileFactory.CreateFromFile("IntegrationTests/TestProcess/test-process.zip");
        public static FormFile EmptyArchive => FormFileFactory.CreateFromFile("IntegrationTests/TestProcess/empty-archive.zip");
        public static FormFile InvalidProcessYaml => FormFileFactory.CreateFromFile("IntegrationTests/TestProcess/invalid-process-yaml.zip");
    }
}