using Autofac;
using SheepIt.Api.Core.Environments.Queries;

namespace SheepIt.Api.Core.Environments
{
    public class EnvironmentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AddEnvironment>().AsSelf();
            
            builder.RegisterType<GetEnvironmentsQuery>().AsSelf();
        }
    }
}