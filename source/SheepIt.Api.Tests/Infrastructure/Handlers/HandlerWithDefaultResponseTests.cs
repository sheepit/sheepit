using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Tests.Infrastructure.Handlers
{
    public class HandlerWithDefaultResponseTests
    {
        [Test]
        public async Task can_decorate_a_handler_without_response_to_return_default_response()
        {
            // given

            var handlerMock = new Mock<IHandler<RequestWithNoResponse>>();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(handlerMock.Object)
                .As<IHandler<RequestWithNoResponse>>();

            BuildRegistration.Instance(handlerMock.Object)
                .WithDefaultResponse()
                .RegisterHandlerIn(builder);
            
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

                handlerMock.Verify(handler => handler.Handle(request), Times.Once);
            }
        }

        public class RequestWithNoResponse : IRequest
        {
        }
    }
}