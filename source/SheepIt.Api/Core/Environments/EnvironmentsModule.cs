using Autofac;

namespace SheepIt.Api.Core.Environments
{
    public class EnvironmentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AddEnvironment>().AsSelf();
        }
    }
}