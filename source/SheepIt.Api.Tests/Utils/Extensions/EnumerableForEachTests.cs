using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.Tests.Utils.Extensions
{
    public class EnumerableForEachTests
    {
        [Test]
        public void can_iterate_an_enumerable_items()
        {
            // given
            
            var items = new[] { "zero", "one", "two" };
            
            // when

            var results = new List<string>();
            
            // then
            
            items.ForEach(item => results.Add(item));

            results.Should().Equal("zero", "one", "two");
        }
        
        [Test]
        public void can_iterate_an_enumerable_items_with_their_indexes()
        {
            // given
            
            var items = new[] { "zero", "one", "two" };
            
            // when

            var results = new List<(string, int)>();
            
            // then
            
            items.ForEach((item, index) => results.Add((item, index)));

            results.Should().Equal(("zero", 0), ("one", 1), ("two", 2));
        }
    }
}