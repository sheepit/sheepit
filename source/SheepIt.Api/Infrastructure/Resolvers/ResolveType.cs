using Autofac;

namespace SheepIt.Api.Infrastructure.Resolvers
{
    public class ResolveType<TResolved> : IResolver<TResolved>
    {
        public TResolved Resolve(IComponentContext context)
        {
            return context.Resolve<TResolved>();
        }
    }
}