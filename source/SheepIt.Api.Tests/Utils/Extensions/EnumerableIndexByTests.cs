using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Utils;

// ReSharper disable NotAccessedField.Local - it's accessed by fluent assertions

namespace SheepIt.Api.Tests.Utils.Extensions
{
    public class EnumerableIndexByTests
    {
        [Test]
        public void can_index_enumerable_by_a_key()
        {
            // given
            
            var items = new[]
            {
                new SampleItem { Id = 1, Value = "one" },
                new SampleItem { Id = 2, Value = "two" },
                new SampleItem { Id = 3, Value = "three" }
            };
            
            // when

            var itemsById = items.IndexBy(item => item.Id);
            
            // then
            
            itemsById.Should().BeEquivalentTo(new Dictionary<int, SampleItem>
            {
                { 1, new SampleItem { Id = 1, Value = "one" } },
                { 2, new SampleItem { Id = 2, Value = "two" } },
                { 3, new SampleItem { Id = 3, Value = "three" } },
            });
        }

        class SampleItem
        {
            public int Id;
            public string Value;
        }
    }
}