using System.Threading.Tasks;
using Autofac;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Core.ProjectContext
{
    public class ProjectContextHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
        where TRequest : IProjectRequest
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IResolver<IHandler<TRequest, TResponse>> _innerResolver;
        private readonly IProjectLock _projectLock;
        private readonly IProjectContextFactory _projectContextFactory;

        public ProjectContextHandler(ILifetimeScope lifetimeScope, IResolver<IHandler<TRequest, TResponse>> innerResolver, IProjectLock projectLock, IProjectContextFactory projectContextFactory)
        {
            _lifetimeScope = lifetimeScope;
            _innerResolver = innerResolver;
            _projectLock = projectLock;
            _projectContextFactory = projectContextFactory;
        }

        public async Task<TResponse> Handle(TRequest request)
        {
            // todo: [rt] should we really synchronize all actions in project context? what about queries?
            // todo: [ts] I'm not sure if it the best option here - it might influence the overall performance
            // on the other hand the system rather will no be used simultaneously by many users
            using (await _projectLock.LockAsync(request.ProjectId))
            using (var projectScope = await BeginProjectScope(request.ProjectId))
            {
                return await _innerResolver
                    .Resolve(projectScope)
                    .Handle(request);
            }
        }

        private async Task<ILifetimeScope> BeginProjectScope(string projectId)
        {
            var projectContext = await _projectContextFactory.Create(projectId);

            return _lifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(projectContext);
            });
        }
    }

    public class ProjectContextResolver<TRequest, TResponse> : IResolver<IHandler<TRequest, TResponse>>
        where TRequest : IProjectRequest
    {
        private readonly IResolver<IHandler<TRequest, TResponse>> _innerResolver;

        public ProjectContextResolver(IResolver<IHandler<TRequest, TResponse>> innerResolver)
        {
            _innerResolver = innerResolver;
        }

        public IHandler<TRequest, TResponse> Resolve(IComponentContext context)
        {
            var lifetimeScope = context.Resolve<ILifetimeScope>();
            
            return new ProjectContextHandler<TRequest, TResponse>(
                innerResolver: _innerResolver,
                lifetimeScope: lifetimeScope,
                projectLock: new ProjectLock(),
                projectContextFactory: context.Resolve<IProjectContextFactory>()
            );
        }
    }

    public static class ProjectContextHandler
    {
        public static IResolver<IHandler<TRequest, TResponse>> InProjectContext<TRequest, TResponse>(
            this IResolver<IHandler<TRequest, TResponse>> innerResolver)
            where TRequest : IProjectRequest
        {
            return new ProjectContextResolver<TRequest, TResponse>(innerResolver);
        }
    }
    
    public interface IProjectRequest
    {
        string ProjectId { get; }
    }
}