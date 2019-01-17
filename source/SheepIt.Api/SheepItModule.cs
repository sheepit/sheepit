using Autofac;
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
            
            
            
            builder.RegisterType<CreateProjectHandler>().AsSelf();
            builder.RegisterType<DeployReleaseHandler>().AsSelf();
            builder.RegisterType<ListProjectsHandler>().AsSelf();
            builder.RegisterType<ShowDashboardHandler>().AsSelf();
            
            builder.RegisterType<GetDeploymentDetailsHandler>().AsSelf();
            builder.RegisterType<GetDeploymentUsedVariablesHandler>().AsSelf();
            builder.RegisterType<ListDeploymentsHandler>().AsSelf();
            
            builder.RegisterType<ListEnvironmentsHandler>().AsSelf();
            builder.RegisterType<UpdateEnvironmentsRankHandler>().AsSelf();
            
            builder.RegisterType<EditReleaseVariablesHandler>().AsSelf();
            builder.RegisterType<GetLastReleaseHandler>().AsSelf();
            builder.RegisterType<GetReleaseDetailsHandler>().AsSelf();
            builder.RegisterType<ListReleasesHandler>().AsSelf();
            builder.RegisterType<UpdateReleaseProcessHandler>().AsSelf();
            builder.RegisterType<UpdateReleaseVariablesHandler>().AsSelf();
        }
    }
}