namespace SheepIt.Api.Infrastructure.Resolvers
{
    public static class BuildRegistration
    {
        public static IResolver<TResolved> Type<TResolved>()
        {
            return new ResolveType<TResolved>();
        }

        public static IResolver<TResolved> Instance<TResolved>(TResolved instance)
        {
            return new ResolveInstance<TResolved>(instance);
        }
    }
}