using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;

namespace SheepIt.Api.Tests.TestInfrastructure
{
    public class IntegrationTestsFixture : IFixture
    {
        private IContainer _container;
        
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterModule<SheepItModule>();
            builder.RegisterModule<TestConfiguration>();

            _container = builder.Build();

            ClearDatabase();
        }

        private void ClearDatabase()
        {
            using var dbContext = CreateSheepItDbContext();
            
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
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
            
            var dbContextOptions = new DbContextOptionsBuilder<SheepItDbContext>()
                .UseNpgsql(configuration.GetConnectionString(SheepItDbContext.ConnectionStringName))
                .Options;
            
            return new SheepItDbContext(dbContextOptions);
        }

        public TResolved Resolve<TResolved>() => _container.Resolve<TResolved>();

        public void TearDown()
        {
            _container?.Dispose();
        }
    }
}