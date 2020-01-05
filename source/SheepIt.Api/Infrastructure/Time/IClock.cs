using System;
using Autofac;

namespace SheepIt.Api.Infrastructure.Time
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }

    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    public class TimeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SystemClock>()
                .As<IClock>()
                .SingleInstance();
        }
    }
}