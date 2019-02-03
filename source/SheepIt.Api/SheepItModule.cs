using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.Core.DeploymentProcessRunning;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Mongo;
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

            RegisterHandlers(builder);
        }

        private void RegisterHandlers(ContainerBuilder builder)
        {
            // Dashboard
            builder.RegisterModule<ShowDashboardModule>();

            // Project
            builder.RegisterModule<CreateProjectModule>();
            builder.RegisterModule<ListProjectsModule>();
            
            // Deployment
            builder.RegisterModule<GetDeploymentDetailsModule>();
            builder.RegisterModule<GetDeploymentUsedVariablesModule>();
            builder.RegisterModule<ListDeploymentsModule>();
            
            // Environment
            builder.RegisterModule<ListEnvironmentsModule>();
            builder.RegisterModule<UpdateEnvironmentsRankModule>();
            
            // Release
            builder.RegisterModule<DeployReleaseModule>();
            builder.RegisterModule<EditReleaseVariablesModule>();
            builder.RegisterModule<GetLastReleaseModule>();
            builder.RegisterModule<GetReleaseDetailsModule>();
            builder.RegisterModule<ListReleasesModule>();
            builder.RegisterModule<UpdateReleaseProcessModule>();
            builder.RegisterModule<UpdateReleaseVariablesModule>();
        }
    }
}