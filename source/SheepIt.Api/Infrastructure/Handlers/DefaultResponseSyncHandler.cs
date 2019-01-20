using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Infrastructure.Handlers
{
    public class DefaultResponseSyncHandler<TRequest> : ISyncHandler<TRequest, Nothing>
    {
        private readonly ISyncHandler<TRequest> _innerHandler;

        public DefaultResponseSyncHandler(ISyncHandler<TRequest> innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public Nothing Handle(TRequest request)
        {
            _innerHandler.Handle(request);

            return Nothing.Value;
        }
    }

    public static class NoResultSyncHandlerExtensions
    {
        public static IResolver<ISyncHandler<TRequest, Nothing>> WithDefaultResponse<TRequest>(
            this IResolver<ISyncHandler<TRequest>> innerResolver)
        {
            return innerResolver.Select(handler => new DefaultResponseSyncHandler<TRequest>(handler));
        }
    }
}