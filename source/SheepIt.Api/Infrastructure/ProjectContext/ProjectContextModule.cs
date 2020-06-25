using Autofac;

namespace SheepIt.Api.Infrastructure.ProjectContext
{
    public class ProjectContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // since this is a lock, it's important, that it's registered as a single instance
            builder.RegisterType<ProjectLock>()
                .As<IProjectLock>()
                .SingleInstance();
        }
    }
}