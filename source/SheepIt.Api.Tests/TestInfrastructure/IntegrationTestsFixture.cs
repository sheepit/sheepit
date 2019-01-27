using System.Threading.Tasks;
using Autofac;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Domain;

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

            DropDatabase();
        }

        private void DropDatabase()
        {
            var databaseName = Database.MongoDatabase.DatabaseNamespace.DatabaseName;

            Database.MongoClient.DropDatabase(databaseName);
        }

        private SheepItDatabase Database => _container.Resolve<SheepItDatabase>();
        
        public async Task<TResponse> Handle<TResponse>(IRequest<TResponse> request)
        {
            var mediator = _container.Resolve<HandlerMediator>();

            return await mediator.Handle(request);
        }

        public void TearDown()
        {
            _container?.Dispose();
        }
    }
}