using Autofac;
using SheepIt.Api.Core.Packages;
using SheepIt.Api.Core.Packages.CreatePackage;

namespace SheepIt.Api.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreatePackageCommandHandler>().InstancePerDependency();
        }   
    }
}