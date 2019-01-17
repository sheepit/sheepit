using System.Threading.Tasks;

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

    public static class ToAsyncHandlerExtensions
    {
        public static SyncToAsyncHandler<TRequest, TResponse> ToAsyncHandler<TRequest, TResponse>(
            this ISyncHandler<TRequest, TResponse> innerSyncHandler)
        {
            return new SyncToAsyncHandler<TRequest, TResponse>(innerSyncHandler);
        }
    }
}