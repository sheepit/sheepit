using NUnit.Framework;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    public class Test<TFixture>
        where TFixture : IFixture, new()
    {
        public TFixture Fixture { get; private set; }
        
        [SetUp]
        public void SetUp()
        {
            Fixture = new TFixture();
            
            Fixture.SetUp();
        }

        [TearDown]
        public void TearDown()
        {
            Fixture.TearDown();
        }
    }

    public interface IFixture
    {
        void SetUp();
        void TearDown();
    }
}