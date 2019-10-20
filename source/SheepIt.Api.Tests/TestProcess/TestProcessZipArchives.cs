using System.IO;
using Microsoft.AspNetCore.Http.Internal;
using SheepIt.Api.Tests.TestInfrastructure;

namespace SheepIt.Api.Tests.TestProcess
{
    public static class TestProcessZipArchives
    {
        public static FormFile TestProcess => FormFileFactory.CreateFromFile("TestProcess/test-process.zip");
        public static FormFile EmptyArchive => FormFileFactory.CreateFromFile("TestProcess/empty-archive.zip");
        public static FormFile InvalidProcessYaml => FormFileFactory.CreateFromFile("TestProcess/invalid-process-yaml.zip");
    }
}