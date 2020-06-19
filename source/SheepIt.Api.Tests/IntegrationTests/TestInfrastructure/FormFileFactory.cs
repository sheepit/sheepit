using System.IO;
using Microsoft.AspNetCore.Http;

namespace SheepIt.Api.Tests.IntegrationTests.TestInfrastructure
{
    public static class FormFileFactory
    {
        public static FormFile CreateFromFile(string path)
        {
            var bytes = File.ReadAllBytes(path);

            return CreateFromByteArray(bytes);
        }
        
        public static FormFile CreateFromByteArray(byte[] bytes)
        {
            // memory stream is not disposed, because it's needed later for reading FormFile;
            // despite implementing IDisposable, it doesn't need to be disposed anyway, see:
            // https://stackoverflow.com/a/4274769
            var memoryStream = new MemoryStream(bytes);
            
            return new FormFile(
                baseStream: memoryStream,
                baseStreamOffset: 0,
                length: memoryStream.Length,
                name: "some name",
                fileName: "som-file-name.ext"
            );
        }
    }
}