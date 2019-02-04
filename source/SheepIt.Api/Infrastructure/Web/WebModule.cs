using Autofac;

namespace SheepIt.Api.Infrastructure.Web
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WebApp>().AsSelf();
        }
    }
}