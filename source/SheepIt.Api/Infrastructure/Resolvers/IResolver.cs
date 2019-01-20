using Autofac;

namespace SheepIt.Api.Infrastructure.Resolvers
{
    public interface IResolver<out TResolved>
    {
        TResolved Resolve(IComponentContext context);
    }
}