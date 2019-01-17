namespace SheepIt.Api.Infrastructure.Handlers
{
    public class NoResultSyncHandler<TRequest> : ISyncHandler<TRequest, Nothing>
    {
        private readonly ISyncHandler<TRequest> _innerHandler;

        public NoResultSyncHandler(ISyncHandler<TRequest> innerHandler)
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
        public static NoResultSyncHandler<TRequest> WithNoResult<TRequest>(this ISyncHandler<TRequest> innerSyncHandler)
        {
            return new NoResultSyncHandler<TRequest>(innerSyncHandler);
        }
    }
}