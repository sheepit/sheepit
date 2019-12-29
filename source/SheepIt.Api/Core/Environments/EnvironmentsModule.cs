using Autofac;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.Projects;

namespace SheepIt.Api.Core.Environments
{
    public class EnvironmentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EnvironmentRepository>().AsSelf();

            builder.RegisterType<AddEnvironment>().AsSelf();
            
            builder.RegisterType<GetEnvironmentsCountQuery>().AsSelf();
            builder.RegisterType<GetEnvironmentsQuery>().AsSelf();
        }
    }
}