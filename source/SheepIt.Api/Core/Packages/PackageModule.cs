using Autofac;

namespace SheepIt.Api.Core.Packages
{
    public class PackageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PackageFactory>();
        }
    }
}