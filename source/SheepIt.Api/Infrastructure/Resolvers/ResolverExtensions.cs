using Autofac;
using Autofac.Builder;

namespace SheepIt.Api.Infrastructure.Resolvers
{
    public static class ResolverExtensions
    {
        public static IRegistrationBuilder<TResolved, SimpleActivatorData, SingleRegistrationStyle> RegisterIn<TResolved>(
            this IResolver<TResolved> resolver, ContainerBuilder builder)
        {
            return builder.Register(resolver.Resolve);
        }
    }
}