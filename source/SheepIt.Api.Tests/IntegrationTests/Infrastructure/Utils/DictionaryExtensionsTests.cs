using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Utils;

namespace SheepIt.Api.Tests.IntegrationTests.Infrastructure.Utils
{
    public class DictionaryExtensionsTests
    {
        [Test]
        public void can_copy_a_dictionary()
        {
            // given
            
            var originalDictionary = new Dictionary<int, string>
            {
                {1, "a"},
                {2, "b"},
                {3, "c"}
            };
            
            // when

            var copiedDictionary = originalDictionary.ToDictionary();
            
            // then

            copiedDictionary.Should().NotBeSameAs(originalDictionary);
            copiedDictionary.Should().BeEquivalentTo(originalDictionary);
        }
    }
}