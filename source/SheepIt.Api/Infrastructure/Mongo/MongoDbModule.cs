using Autofac;
using Microsoft.Extensions.Configuration;

namespace SheepIt.Api.Infrastructure.Mongo
{
    public class MongoDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => context
                .Resolve<IConfiguration>()
                .GetSection("Mongo")
                .Get<MongoSettings>()
            );

            builder.RegisterType<SheepItDatabase>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<IdentityProvider>()
                .InstancePerLifetimeScope();
        }
    }
}