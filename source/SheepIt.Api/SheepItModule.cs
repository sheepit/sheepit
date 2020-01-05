using Autofac;
using Autofac.Features.ResolveAnything;
using SheepIt.Api.Core.DeploymentProcesses;
using SheepIt.Api.Core.DeploymentProcessRunning;
using SheepIt.Api.Core.Deployments;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.Infrastructure.Authorization;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Time;
using SheepIt.Api.Infrastructure.Web;
using SheepIt.Api.UseCases.Dashboard;
using SheepIt.Api.UseCases.ProjectManagement;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;
using SheepIt.Api.UseCases.ProjectOperations.Environments;
using SheepIt.Api.UseCases.ProjectOperations.Packages;
using SystemClock = Microsoft.Extensions.Internal.SystemClock;

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
            builder.RegisterModule<ErrorHandlingModule>();
            builder.RegisterModule<MongoDbModule>();
            builder.RegisterModule<SheepItAuthenticationModule>();
            builder.RegisterModule<TimeModule>();
        }

        private void RegisterCore(ContainerBuilder builder)
        {
            builder.RegisterModule<EnvironmentsModule>();
            builder.RegisterModule<ProjectContextModule>();
            builder.RegisterModule<PackageModule>();
            
            builder.RegisterType<DeploymentsStorage>().AsSelf();
            builder.RegisterType<PackageRepository>().AsSelf();
            builder.RegisterType<DeploymentProcess>().AsSelf();
            builder.RegisterModule<DeploymentProcessModule>();
        }

        private void RegisterUseCases(ContainerBuilder builder)
        {
            // Dashboard
            builder.RegisterModule<GetDashboardModule>();

            // Project
            builder.RegisterModule<CreateProjectModule>();
            builder.RegisterModule<GetProjectDashboardModule>();
            builder.RegisterModule<GetEnvironmentsForUpdateModule>();
            builder.RegisterModule<ListProjectsModule>();
            builder.RegisterModule<UpdateEnvironmentsModule>();
            builder.RegisterType<ProjectRepository>();
            builder.RegisterType<GetProjectsListQuery>();
            
            // Deployment
            builder.RegisterModule<GetDeploymentDetailsModule>();
            builder.RegisterModule<ListPackageDeploymentsModule>();
            
            // Environment
            builder.RegisterModule<ListEnvironmentsModule>();
            
            // Package
            builder.RegisterModule<DeployPackageModule>();
            builder.RegisterModule<EditPackageVariablesModule>();
            builder.RegisterModule<GetLastPackageModule>();
            builder.RegisterModule<GetPackageDetailsModule>();
            builder.RegisterModule<CreatePackageModule>();
        }
    }
}
