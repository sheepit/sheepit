using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.CommandRunners;
using SheepIt.Api.Infrastructure;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.UseCases;
using SheepIt.Api.UseCases.Deployments;
using SheepIt.Api.UseCases.Environments;
using SheepIt.Api.UseCases.Releases;
using SheepIt.Domain;

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

            builder.RegisterType<ProcessSettings>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<ProcessRepositoryFactory>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<ProcessRunner>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ShellSettings>()
                .AsSelf()
                .SingleInstance();

            // this is mainly used to resolve handlers before decorating them
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            
            builder.RegisterType<Deployments>().AsSelf();
            builder.RegisterType<Environments>().AsSelf();
            builder.RegisterType<Projects>().AsSelf();
            builder.RegisterType<ReleasesStorage>().AsSelf();
            
            // todo: move into specific handlers

            BuildRegistration.Type<CreateProjectHandler>()
                .WithDefaultResponse()
                .AsAsyncHandler()
                .RegisterIn(builder);
            
            BuildRegistration.Type<ListProjectsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);
            
            BuildRegistration.Type<ShowDashboardHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<DeployReleaseHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

            BuildRegistration.Type<GetDeploymentDetailsHandler>()
                .AsAsyncHandler()
                .RegisterIn(builder);

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
    }
}