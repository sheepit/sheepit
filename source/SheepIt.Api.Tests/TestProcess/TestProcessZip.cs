using System.IO;
using Microsoft.AspNetCore.Http.Internal;

namespace SheepIt.Api.Tests.TestProcess
{
    public static class TestProcessZip
    {
        public static FormFile GetAsFromFile()
        {
            var zipFileBytes = File.ReadAllBytes("TestProcess/test-process.zip");

            return new FormFile(
                baseStream: new MemoryStream(zipFileBytes),
                baseStreamOffset: 0,
                length: 0,
                name: "Data",
                fileName: "process.zip"
            );
        }
    }
}