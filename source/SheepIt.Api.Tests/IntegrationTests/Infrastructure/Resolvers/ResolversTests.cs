using Autofac;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Tests.IntegrationTests.Infrastructure.Resolvers
{
    public class ResolversTests
    {
        [Test]
        public void can_resolve_a_type()
        {
            // given
            
            var builder = new ContainerBuilder();

            builder.RegisterType<SomeType>();

            BuildRegistration.Type<SomeType>()
                .RegisterIn(builder)
                .As<ISomeInterface>();

            using (var container = builder.Build())
            {
                // when
                
                var result = container.Resolve<ISomeInterface>();
                
                // then

                result.Should().BeOfType<SomeType>();
            }
        }

        [Test]
        public void can_resolve_an_instance()
        {
            // given
            
            var builder = new ContainerBuilder();

            var someInstance = new SomeType();
            
            BuildRegistration.Instance(someInstance)
                .RegisterIn(builder);

            using (var container = builder.Build())
            {
                // when

                var result = container.Resolve<SomeType>();
                
                // then

                result.Should().BeSameAs(someInstance);
            }
        }

        [Test]
        public void can_map_a_resolved_type()
        {
            // given
            
            var builder = new ContainerBuilder();

            var someInstance = new SomeType();

            BuildRegistration.Instance(someInstance)
                .Select(someType => new SomeTypeWrapper { SomeType = someType })
                .RegisterIn(builder);

            using (var container = builder.Build())
            {
                // when

                var result = container.Resolve<SomeTypeWrapper>();
                
                // then

                result.SomeType.Should().BeSameAs(someInstance);
            }
        }

        public class SomeTypeWrapper
        {
            public SomeType SomeType { get; set; }
        }

        public class SomeType : ISomeInterface
        {
        }
        
        public interface ISomeInterface
        {
        }
    }
}