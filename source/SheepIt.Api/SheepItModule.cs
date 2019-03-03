using Autofac;
using Autofac.Features.ResolveAnything;
using SheepIt.Api.Core.DeploymentProcessRunning;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Core.Releases;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.UseCases.ProjectManagement;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;
using SheepIt.Api.UseCases.ProjectOperations.Environments;
using SheepIt.Api.UseCases.ProjectOperations.Releases;

namespace SheepIt.Api
{
    public class SheepItModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // this is mainly used to resolve handlers before decorating them
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            
            RegisterInfrastructure(builder);
            RegisterCore(builder);
            RegisterUseCases(builder);
        }

        private static void RegisterInfrastructure(ContainerBuilder builder)
        {
            builder.RegisterModule<WebModule>();
            builder.RegisterModule<MongoDbModule>();
        }

        private void RegisterCore(ContainerBuilder builder)
        {
            builder.RegisterModule<EnvironmentsModule>();
            builder.RegisterModule<ProjectContextModule>();
            
            builder.RegisterType<DeploymentsStorage>().AsSelf();
            builder.RegisterType<ReleasesStorage>().AsSelf();
            builder.RegisterModule<DeploymentProcessModule>();
        }

        private void RegisterUseCases(ContainerBuilder builder)
        {
            // Dashboard
            builder.RegisterModule<ShowDashboardModule>();

            // Project
            builder.RegisterModule<CreateProjectModule>();
            builder.RegisterModule<GetProjectDetailsModule>();
            builder.RegisterModule<ListProjectsModule>();
            builder.RegisterModule<UpdateProjectModule>();
            
            // Deployment
            builder.RegisterModule<GetDeploymentDetailsModule>();
            builder.RegisterModule<ListReleaseDeploymentsModule>();
            
            // Environment
            builder.RegisterModule<ListEnvironmentsModule>();
            builder.RegisterModule<UpdateEnvironmentDisplayNameModule>();
            builder.RegisterModule<UpdateEnvironmentsRankModule>();
            builder.RegisterModule<AddEnvironmentModule>();
            
            // Release
            builder.RegisterModule<DeployReleaseModule>();
            builder.RegisterModule<EditReleaseVariablesModule>();
            builder.RegisterModule<GetLastReleaseModule>();
            builder.RegisterModule<GetReleaseDetailsModule>();
            builder.RegisterModule<UpdateReleaseProcessModule>();
            builder.RegisterModule<UpdateReleaseVariablesModule>();
        }
    }
}
