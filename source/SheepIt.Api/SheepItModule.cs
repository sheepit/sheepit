using Autofac;
using Autofac.Features.ResolveAnything;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Authorization;
using SheepIt.Api.Infrastructure.ErrorHandling;
using SheepIt.Api.Infrastructure.ProjectContext;
using SheepIt.Api.Infrastructure.Time;
using SheepIt.Api.Runner.DeploymentProcessRunning;
using SheepIt.Api.UseCases.Dashboard;
using SheepIt.Api.UseCases.ProjectManagement;
using SheepIt.Api.UseCases.ProjectOperations.Components;
using SheepIt.Api.UseCases.ProjectOperations.Dashboard;
using SheepIt.Api.UseCases.ProjectOperations.DeploymentDetails;
using SheepIt.Api.UseCases.ProjectOperations.Deployments;
using SheepIt.Api.UseCases.ProjectOperations.Environments;
using SheepIt.Api.UseCases.ProjectOperations.Packages;

namespace SheepIt.Api
{
    public class SheepItModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // this is mainly used to resolve handlers before decorating them
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            builder.RegisterModule<DataAccessModule>();
            
            RegisterInfrastructure(builder);
            RegisterDataAccess(builder);
            RegisterRunner(builder);
            RegisterUseCases(builder);
            RegisterPublicApi(builder);
        }

        private static void RegisterDataAccess(ContainerBuilder builder)
        {
            builder.RegisterModule<DataAccessModule>();
        }

        private static void RegisterInfrastructure(ContainerBuilder builder)
        {
            builder.RegisterModule<ErrorHandlingModule>();
            builder.RegisterModule<SheepItAuthenticationModule>();
            builder.RegisterModule<TimeModule>();
            builder.RegisterModule<ProjectContextModule>();
        }

        private void RegisterRunner(ContainerBuilder builder)
        {
            builder.RegisterModule<DeploymentProcessModule>();
        }

        private void RegisterUseCases(ContainerBuilder builder)
        {
            // Dashboard
            builder.RegisterModule<GetDashboardModule>();

            // Project
            builder.RegisterModule<GetProjectDashboardModule>();
            builder.RegisterModule<CreateProjectModule>();
            builder.RegisterModule<GetEnvironmentsForUpdateModule>();
            builder.RegisterModule<ListProjectsModule>();
            builder.RegisterModule<UpdateEnvironmentsModule>();
            
            // Deployment
            builder.RegisterModule<GetDeploymentDetailsModule>();
            builder.RegisterModule<GetDeploymentCurlModule>();
            builder.RegisterModule<ListPackageDeploymentsModule>();
            builder.RegisterModule<GetDeploymentsListModule>();
            
            // Environment
            builder.RegisterModule<ListEnvironmentsModule>();
            builder.RegisterModule<GetEnvironmentsListModule>();

            // Package
            builder.RegisterModule<DeployPackageModule>();
            builder.RegisterModule<GetLastPackageModule>();
            builder.RegisterModule<GetPackagesListModule>();
            builder.RegisterModule<GetPackageDetailsModule>();
            builder.RegisterModule<CreatePackageModule>();
            
            // Component
            builder.RegisterModule<ListComponentsModule>();
            builder.RegisterModule<UpdateComponentsModule>();
            builder.RegisterModule<GetComponentsForUpdateModule>();
        }

        private void RegisterPublicApi(ContainerBuilder builder)
        {
            // Package
            builder.RegisterModule<PublicApi.Packages.CreatePackageModule>();
            builder.RegisterModule<PublicApi.Packages.UploadPackageModule>();
        }
    }
}
