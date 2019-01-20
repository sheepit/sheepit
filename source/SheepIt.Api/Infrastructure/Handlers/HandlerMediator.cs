using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;

namespace SheepIt.Api.Infrastructure.Handlers
{
    public class HandlerMediator
    {
        private readonly ILifetimeScope _scope;

        public HandlerMediator(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public async Task<TResponse> Handle<TResponse>(IRequest<TResponse> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await (Task<TResponse>) GetType()
                .GetMethod(nameof(Handle), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(request.GetType(), typeof(TResponse))
                .Invoke(this, new object[] { request });
        }

        private async Task<TResponse> Handle<TRequest, TResponse>(TRequest request)
        {
            var handler = ResolveHandler<TRequest, TResponse>();

            return await handler.Handle(request);
        }

        private IHandler<TRequest, TResponse> ResolveHandler<TRequest, TResponse>()
        {
            try
            {
                return _scope.Resolve<IHandler<TRequest, TResponse>>();
            }
            catch (Exception exception)
            {
                var requestName = typeof(TRequest).Name;
                var responseName = typeof(TResponse).Name;
                var handlerName = $"IHandler<{requestName}, {responseName}>";

                throw new InvalidOperationException(
                    message: $"Mediator couldn't handle {requestName} because {handlerName} could not be resolved.",
                    innerException: exception
                );
            }
        }
    }
}