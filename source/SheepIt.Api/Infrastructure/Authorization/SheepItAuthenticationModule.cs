using Autofac;
using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Infrastructure.Authorization
{
    public class SheepItAuthenticationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SingleUserAuthentication>()
                .AsSelf()
                .SingleInstance();

            builder.Register(context => 
                SingleUserAuthenticationSettings.FromConfiguration(
                    context.Resolve<IConfiguration>()
                )
            );
        }
    }
}