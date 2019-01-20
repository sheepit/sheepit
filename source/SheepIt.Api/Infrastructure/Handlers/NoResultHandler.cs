using System.Threading.Tasks;

namespace SheepIt.Api.Infrastructure.Handlers
{
    public class NoResultHandler<TRequest> : IHandler<TRequest, Nothing>
    {
        private readonly IHandler<TRequest> _innerHandler;

        public NoResultHandler(IHandler<TRequest> innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public async Task<Nothing> Handle(TRequest request)
        {
            await _innerHandler.Handle(request);

            return Nothing.Value;
        }
    }

    public static class NoResultHandlerExtensions
    {
        // todo: with default response
        public static NoResultHandler<TRequest> WithNoResult<TRequest>(this IHandler<TRequest> innerHandler)
        {
            return new NoResultHandler<TRequest>(innerHandler);
        }
    }
}