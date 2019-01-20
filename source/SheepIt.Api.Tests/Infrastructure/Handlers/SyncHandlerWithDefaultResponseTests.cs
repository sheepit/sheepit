using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Tests.Infrastructure.Handlers
{
    public class SyncHandlerWithDefaultResponseTests
    {
        [Test]
        public async Task can_decorate_a_handler_without_response_to_return_default_response()
        {
            // given

            var syncHandlerMock = new Mock<ISyncHandler<RequestWithNoResponse>>();

            var builder = new ContainerBuilder();

            BuildRegistration.Instance(syncHandlerMock.Object)
                .WithDefaultResponse()
                .AsAsyncHandler()
                .RegisterAsHandlerIn(builder);
            
            builder.RegisterType<HandlerMediator>()
                .AsSelf();

            using (var container = builder.Build())
            {
                var request = new RequestWithNoResponse();

                // when

                var response = await container
                    .Resolve<HandlerMediator>()
                    .Handle(request);

                // then

                response.Should().BeSameAs(Nothing.Value);

                syncHandlerMock.Verify(handler => handler.Handle(request), Times.Once);
            }
        }

        public class RequestWithNoResponse : IRequest
        {
        }
    }
}