using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;

namespace SheepIt.Api.Tests.IntegrationTests.TestInfrastructure
{
    public class IntegrationTestsFixture : IFixture
    {
        private IContainer _container;
        
        public async Task SetUp()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterModule<SheepItModule>();
            builder.RegisterModule<TestConfiguration>();
            builder.RegisterModule<TestClockModule>();

            _container = builder.Build();

            await ClearDatabase();
            TestWorkingDir.Clear(this);
        }

        private async Task ClearDatabase()
        {
            await using var dbContext = CreateSheepItDbContext();
            
            await TestDatabase.TruncateDatabase(dbContext);
            await TestDatabase.ResetSequences(dbContext);
        }

        public async Task<TResponse> Handle<TResponse>(IRequest<TResponse> request)
        {
            using var scope = BeginDbContextScope();
            
            var mediator = scope.Resolve<HandlerMediator>();
                
            return await mediator.Handle(request);
        }

        public ILifetimeScope BeginDbContextScope()
        {
            return _container.BeginLifetimeScope(builder =>
            {
                builder.Register(context => CreateSheepItDbContext())
                    .SingleInstance()
                    .AsSelf();
            });
        }

        private SheepItDbContext CreateSheepItDbContext()
        {
            var configuration = _container.Resolve<IConfiguration>();

            return TestDatabase.CreateSheepItDbContext(configuration);
        }

        public TResolved Resolve<TResolved>() => _container.Resolve<TResolved>();

        public Task TearDown()
        {
            _container?.Dispose();

            return Task.CompletedTask;
        }
    }
}