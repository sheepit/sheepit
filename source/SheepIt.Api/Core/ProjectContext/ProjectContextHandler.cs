using System.Threading.Tasks;
using Autofac;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Core.ProjectContext
{
    public class ProjectContextHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
        where TRequest : IProjectRequest
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly SheepItDatabase _database;
        private readonly IResolver<IHandler<TRequest, TResponse>> _innerResolver;

        public ProjectContextHandler(ILifetimeScope lifetimeScope, SheepItDatabase database, IResolver<IHandler<TRequest, TResponse>> innerResolver)
        {
            _lifetimeScope = lifetimeScope;
            _database = database;
            _innerResolver = innerResolver;
        }

        public Task<TResponse> Handle(TRequest request)
        {
            // todo: [rt] better diagnostics?
            var project = _database.Projects.FindById(request.ProjectId);

            using (var projectScope = BeginProjectScope(project))
            {
                // todo: [rt] lock operations on a given project

                return _innerResolver
                    .Resolve(projectScope)
                    .Handle(request);
            }
        }

        private ILifetimeScope BeginProjectScope(Project project)
        {
            return _lifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance<IProjectContext>(new ProjectContext(project));
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
                database: context.Resolve<SheepItDatabase>()
            );
        }
    }

    public static class ProjectContextHandler
    {
        public static IResolver<IHandler<TRequest, TResponse>> InContextOfProject<TRequest, TResponse>(
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