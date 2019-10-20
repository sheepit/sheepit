using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.Tests.TestInfrastructure;

namespace SheepIt.Api.Tests.Infrastructure.Web
{
    public class FormFileExtensionsTests
    {
        [Test]
        public async Task can_convert_form_file_to_byte_array()
        {
            var originalBytes = Enumerable
                .Repeat((byte) 1, 100)
                .ToArray();
            
            var formFile = FormFileFactory.CreateFromByteArray(originalBytes);

            // when
            
            var resultBytes = await formFile.ToByteArray();
            
            // then

            resultBytes.Should().Equal(originalBytes);
        }
    }
}