using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SheepIt.Api.Tests.IntegrationTests.TestInfrastructure;

namespace SheepIt.Api.Tests.IntegrationTests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            var configuration = TestConfigurationFactory.Build();

            await using var dbContext = TestDatabase.CreateSheepItDbContext(configuration);
        
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
        }
    }
}