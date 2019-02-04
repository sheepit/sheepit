using Autofac;
using SheepIt.Api.Core.DeploymentProcessRunning.DeploymentProcessAccess;

namespace SheepIt.Api.Core.DeploymentProcessRunning
{
    public class DeploymentProcessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DeploymentProcessSettings>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<ShellSettings>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<DeploymentProcessGitRepositoryFactory>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<DeploymentProcessRunner>()
                .AsSelf()
                .SingleInstance();
        }
    }
}