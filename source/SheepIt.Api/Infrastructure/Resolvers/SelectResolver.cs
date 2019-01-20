using System;
using Autofac;

namespace SheepIt.Api.Infrastructure.Resolvers
{
    public class SelectResolver<TInnerResolved, TOuterResolved> : IResolver<TOuterResolved>
    {
        private readonly IResolver<TInnerResolved> _innerResolver;
        private readonly Func<TInnerResolved, TOuterResolved> _selector;

        public SelectResolver(IResolver<TInnerResolved> innerResolver, Func<TInnerResolved, TOuterResolved> selector)
        {
            _innerResolver = innerResolver;
            _selector = selector;
        }

        public TOuterResolved Resolve(IComponentContext context)
        {
            return _selector(_innerResolver.Resolve(context));
        }
    }

    public static class SelectResolverExtensions
    {
        public static SelectResolver<TInnerResolved, TOuterResolved> Select<TInnerResolved, TOuterResolved>(
            this IResolver<TInnerResolved> innerResolver,
            Func<TInnerResolved, TOuterResolved> selector)
        {
            return new SelectResolver<TInnerResolved, TOuterResolved>(innerResolver, selector);
        }
    }
}