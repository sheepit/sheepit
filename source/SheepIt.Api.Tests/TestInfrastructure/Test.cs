using System.Threading.Tasks;
using NUnit.Framework;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    public class Test<TFixture>
        where TFixture : IFixture, new()
    {
        protected TFixture Fixture { get; private set; }
        
        [SetUp]
        public async Task SetUp()
        {
            Fixture = new TFixture();
            
            await Fixture.SetUp();
        }

        [TearDown]
        public async Task TearDown()
        {
            await Fixture.TearDown();
        }
    }

    public interface IFixture
    {
        Task SetUp();
        Task TearDown();
    }
}