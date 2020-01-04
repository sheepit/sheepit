using Autofac;

namespace SheepIt.Api.Core.ProjectContext
{
    public class ProjectContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IProjectLock>(new ProjectLock());
            
            builder.RegisterType<ProjectContextFactory>()
                .As<IProjectContextFactory>()
                .InstancePerDependency();
        }
    }
}