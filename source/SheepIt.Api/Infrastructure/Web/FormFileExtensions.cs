using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SheepIt.Api.Infrastructure.Web
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> ToByteArray(this IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            
            await file.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }
    }
}