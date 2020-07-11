using Autofac;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.DataAccess
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdStorage>().AsSelf();
        }
    }
}