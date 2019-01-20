using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Tests.Infrastructure.Handlers
{
    public class SyncToAsyncHandlerTests
    {
        [Test]
        public async Task can_wrap_sync_handler_with_async_handler()
        {
            // given

            var syncHandlerMock = new Mock<ISyncHandler<Request, Response>>();
            
            var builder = new ContainerBuilder();

            BuildRegistration.Instance(syncHandlerMock.Object)
                .AsAsyncHandler()
                .RegisterAsHandlerIn(builder);
            
            builder.RegisterType<HandlerMediator>()
                .AsSelf();

            using (var container = builder.Build())
            {
                var request = new Request();
                var expectedResponse = new Response();
                
                syncHandlerMock.Setup(handler => handler.Handle(request))
                    .Returns(expectedResponse);

                // when

                var actualResponse = await container
                    .Resolve<HandlerMediator>()
                    .Handle(request);

                // then

                actualResponse.Should().BeSameAs(expectedResponse);

                syncHandlerMock.Verify(handler => handler.Handle(request), Times.Once);
            }
        }
        
        public class Request : IRequest<Response>
        {
        }
        
        public class Response
        {
        }
    }
}