using Autofac;
using SheepIt.Api.Runner.DeploymentProcessRunning.DeploymentProcessAccess;

namespace SheepIt.Api.Runner.DeploymentProcessRunning
{
    public class DeploymentProcessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DeploymentProcessSettings>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<DeploymentProcessDirectoryFactory>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<DeploymentProcessRunner>()
                .AsSelf()
                .SingleInstance();
        }
    }
}