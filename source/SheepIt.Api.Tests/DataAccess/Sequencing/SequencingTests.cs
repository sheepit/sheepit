using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.DataAccess.Sequencing;
using SheepIt.Api.Tests.TestInfrastructure;

namespace SheepIt.Api.Tests.DataAccess.Sequencing
{
    public class SequencingTests : Test<IntegrationTestsFixture>
    {
        [Test]
        public async Task sequence_creates_subsequent_numbers_starting_with_zero()
        {
            using var dbContextScope = Fixture.BeginDbContextScope();
            
            var idStorage = dbContextScope.Resolve<IdStorage>();

            var environmentIds = await GenerateIds(idStorage, count: 25, sequence: IdSequence.Environment);
            var packageIds = await GenerateIds(idStorage, count: 25, sequence: IdSequence.Package);

            environmentIds.Should().Equal(Enumerable.Range(1, 25));
            packageIds.Should().Equal(Enumerable.Range(1, 25));
        }

        private static async Task<List<int>> GenerateIds(IdStorage idStorage, int count, IdSequence sequence)
        {
            var ids = new List<int>();

            for (var index = 0; index < count; index++)
            {
                var nextId = await idStorage.GetNext(sequence);

                ids.Add(nextId);
            }

            return ids;
        }
    }
}