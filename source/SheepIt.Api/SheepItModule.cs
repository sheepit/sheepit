using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Core.DeploymentProcessRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.UseCases;
using SheepIt.Api.UseCases.Deployments;
using SheepIt.Api.UseCases.Environments;
using SheepIt.Api.UseCases.Releases;

namespace SheepIt.Api
{
    public class SheepItModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // todo: divide into many modules
            
            builder.RegisterType<WebApp>().AsSelf();

            builder.Register(context => context
                .Resolve<IConfiguration>()
                .GetSection("Mongo")
                .Get<MongoSettings>()
            );
            
            builder.RegisterType<SheepItDatabase>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<DeploymentProcessSettings>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<DeploymentProcessGitRepositoryFactory>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<DeploymentProcessRunner>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ShellSettings>()
                .AsSelf()
                .SingleInstance();

            // this is mainly used to resolve handlers before decorating them
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            
            builder.RegisterType<DeploymentsStorage>().AsSelf();
            builder.RegisterType<EnvironmentsStorage>().AsSelf();
            builder.RegisterType<ProjectsStorage>().AsSelf();
            builder.RegisterType<ReleasesStorage>().AsSelf();
            
            // todo: move into specific handlers
            RegisterHandlers(builder);
                


            BuildRegistration.Type<GetDeploymentUsedVariablesHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<ListDeploymentsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<ListEnvironmentsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<UpdateEnvironmentsRankHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<EditReleaseVariablesHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<GetLastReleaseHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<GetReleaseDetailsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<ListReleasesHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<UpdateReleaseProcessHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<UpdateReleaseVariablesHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

        }

        private void RegisterHandlers(ContainerBuilder builder)
        {
            builder.RegisterModule<CreateProjectModule>();
            builder.RegisterModule<ListProjectsModule>();
            builder.RegisterModule<DeployReleaseModule>();
            builder.RegisterModule<ShowDashboardModule>();
            builder.RegisterModule<GetDeploymentDetailsModule>();
        }
    }
}