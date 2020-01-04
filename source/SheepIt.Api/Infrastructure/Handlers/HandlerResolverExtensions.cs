using Autofac;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Infrastructure.Handlers
{
    public static class HandlerResolverExtensions
    {
        public static void RegisterAsHandlerIn<TRequest, TResponse>(
            this IResolver<IHandler<TRequest, TResponse>> handlerResolver,
            ContainerBuilder builder)
        {
            handlerResolver.RegisterIn(builder)
                .As<IHandler<TRequest, TResponse>>()
                .InstancePerDependency();
        }
    }
}