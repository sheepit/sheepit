using System.Threading.Tasks;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Infrastructure.Handlers
{
    // todo: this class is temporary, as we should intend for all handlers to be synchronous
    
    public class SyncToAsyncHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
    {
        private readonly ISyncHandler<TRequest, TResponse> _innerSyncHandler;

        public SyncToAsyncHandler(ISyncHandler<TRequest, TResponse> innerSyncHandler)
        {
            _innerSyncHandler = innerSyncHandler;
        }

        public Task<TResponse> Handle(TRequest request)
        {
            var response = _innerSyncHandler.Handle(request);

            return Task.FromResult(response);
        }
    }

    public static class SyncToAsyncHandlerExtensions
    {
        public static IResolver<IHandler<TRequest, TResponse>> AsAsyncHandler<TRequest, TResponse>(
            this IResolver<ISyncHandler<TRequest, TResponse>> innerResolver)
        {
            return innerResolver.Select(handler => new SyncToAsyncHandler<TRequest, TResponse>(handler));
        }
    }
}