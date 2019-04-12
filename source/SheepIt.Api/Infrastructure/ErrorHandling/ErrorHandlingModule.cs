using Autofac;

namespace SheepIt.Api.Infrastructure.ErrorHandling
{
    public class ErrorHandlingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ErrorHandlingSettings>()
                .AsSelf()
                .SingleInstance();
        }
    }
}