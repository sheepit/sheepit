using System.Threading.Tasks;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Infrastructure.Handlers
{
    public class DefaultResponseHandler<TRequest> : IHandler<TRequest, Nothing>
    {
        private readonly IHandler<TRequest> _innerHandler;

        public DefaultResponseHandler(IHandler<TRequest> innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public async Task<Nothing> Handle(TRequest request)
        {
            await _innerHandler.Handle(request);

            return Nothing.Value;
        }
    }

    public static class DefaultResultHandlerExtensions
    {
        public static IResolver<IHandler<TRequest, Nothing>> WithDefaultResponse<TRequest>(
            this IResolver<IHandler<TRequest>> innerResolver)
        {
            return innerResolver.Select(handler => new DefaultResponseHandler<TRequest>(handler));
        }
    }
}