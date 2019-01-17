using Autofac;

namespace SheepIt.Api
{
    public static class SheepItContainer
    {
        public static IContainer Create()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<SheepItModule>();

            return containerBuilder.Build();
        }
    }
}