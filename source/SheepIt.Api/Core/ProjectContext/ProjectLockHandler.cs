using System.Threading.Tasks;
using Autofac;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Core.ProjectContext
{
    public class ProjectLockHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
        where TRequest : IProjectRequest
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IResolver<IHandler<TRequest, TResponse>> _innerResolver;
        private readonly IProjectLock _projectLock;

        public ProjectLockHandler(
            ILifetimeScope lifetimeScope,
            IResolver<IHandler<TRequest, TResponse>> innerResolver,
            IProjectLock projectLock)
        {
            _lifetimeScope = lifetimeScope;
            _innerResolver = innerResolver;
            _projectLock = projectLock;
        }

        public async Task<TResponse> Handle(TRequest request)
        {
            // todo: [rt] should we really synchronize all actions in project context? what about queries?
            // todo: [ts] I'm not sure if it the best option here - it might influence the overall performance
            // on the other hand the system rather will no be used simultaneously by many users
            using (await _projectLock.LockAsync(request.ProjectId))
            {
                return await _innerResolver
                    .Resolve(_lifetimeScope)
                    .Handle(request);
            }
        }
    }

    public class ProjectLockResolver<TRequest, TResponse> : IResolver<IHandler<TRequest, TResponse>>
        where TRequest : IProjectRequest
    {
        private readonly IResolver<IHandler<TRequest, TResponse>> _innerResolver;

        public ProjectLockResolver(IResolver<IHandler<TRequest, TResponse>> innerResolver)
        {
            _innerResolver = innerResolver;
        }

        public IHandler<TRequest, TResponse> Resolve(IComponentContext context)
        {
            var lifetimeScope = context.Resolve<ILifetimeScope>();

            return new ProjectLockHandler<TRequest, TResponse>(
                innerResolver: _innerResolver,
                lifetimeScope: lifetimeScope,
                projectLock: new ProjectLock()
            );
        }
    }

    public static class ProjectLockHandlerExtensions
    {
        public static IResolver<IHandler<TRequest, TResponse>> InProjectLock<TRequest, TResponse>(
            this IResolver<IHandler<TRequest, TResponse>> innerResolver)
            where TRequest : IProjectRequest
        {
            return new ProjectLockResolver<TRequest, TResponse>(innerResolver);
        }
    }
}