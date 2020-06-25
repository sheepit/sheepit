using System;
using Autofac;
using SheepIt.Api.Infrastructure.Time;

namespace SheepIt.Api.Tests.IntegrationTests.TestInfrastructure
{
    public class TestClock : IClock
    {
        public DateTime UtcNow { get; set; } = new DateTime(2019, 9, 13, 12, 00, 00);
    }

    public class TestClockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TestClock>()
                .AsSelf()
                .As<IClock>()
                .SingleInstance();
        }
    }

    public static class TestClockFixtureExtension
    {
        public static void MomentLater(this IntegrationTestsFixture fixture)
        {
            var momentLater = fixture.GetUtcNow().AddMinutes(5);
            
            fixture.SetUtcNow(momentLater);
        }
        
        public static void SetUtcNow(this IntegrationTestsFixture fixture, DateTime dateTime)
        {
            fixture.Resolve<TestClock>().UtcNow = dateTime;
        }

        public static DateTime GetUtcNow(this IntegrationTestsFixture fixture)
        {
            return fixture.Resolve<TestClock>().UtcNow;
        }
    }
}