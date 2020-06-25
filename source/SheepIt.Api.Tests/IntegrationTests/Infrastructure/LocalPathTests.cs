using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure;

namespace SheepIt.Api.Tests.IntegrationTests.Infrastructure
{
    public class LocalPathTests
    {
        [Test]
        public void path_cannot_be_null()
        {
            Action creatingLocalPath = () => new LocalPath(null);

            creatingLocalPath.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void can_add_segment_to_local_path()
        {
            new LocalPath("foo")
                .AddSegment("bar")
                .AddSegment("tar")
                .ToString()
                .Should()
                .Be(Path.Combine("foo", "bar", "tar"));
        }
    }
}