using Autofac;

namespace SheepIt.Api.Infrastructure.Resolvers
{
    public class ResolveInstance<TResolved> : IResolver<TResolved>
    {
        private readonly TResolved _instance;

        public ResolveInstance(TResolved instance)
        {
            _instance = instance;
        }

        public TResolved Resolve(IComponentContext context)
        {
            return _instance;
        }
    }
}