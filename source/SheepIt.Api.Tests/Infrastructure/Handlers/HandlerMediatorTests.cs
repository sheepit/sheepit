using System;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.Tests.Infrastructure.Handlers
{
    public class HandlerMediatorTests
    {
        private IContainer _container;
        private HandlerMediator _mediator;
        
        private Mock<IHandler<RequestWithResponse, Response>> _handlerWithResponseMock;
        private Mock<IHandler<RequestWithNoResponse, Nothing>> _handlerWithNoResponseMock;

        [SetUp]
        public void set_up()
        {
            _handlerWithResponseMock = new Mock<IHandler<RequestWithResponse, Response>>();
            _handlerWithNoResponseMock = new Mock<IHandler<RequestWithNoResponse, Nothing>>();
            
            var builder = new ContainerBuilder();

            BuildRegistration.Instance(_handlerWithResponseMock.Object)
                .RegisterAsHandlerIn(builder);

            BuildRegistration.Instance(_handlerWithNoResponseMock.Object)
                .RegisterAsHandlerIn(builder);

            builder.RegisterType<HandlerMediator>();
            
            _container = builder.Build();

            _mediator = _container.Resolve<HandlerMediator>();
        }

        [TearDown]
        public void tear_down()
        {
            _container?.Dispose();
        }
        
        [Test]
        public async Task mediator_handles_a_request_using_a_registered_handler()
        {
            // given
            
            var request = new RequestWithResponse();

            var expectedResponse = new Response();

            _handlerWithResponseMock.Setup(handler => handler.Handle(request))
                .Returns(Task.FromResult(expectedResponse));
            
            // when
            
            var actualResponse = await _mediator.Handle(request);
            
            // then

            actualResponse.Should().Be(expectedResponse);
            
            _handlerWithResponseMock.Verify(handler => handler.Handle(request), Times.Once);
        }

        [Test]
        public async Task mediator_handles_a_request_without_a_response_using_a_registered_handler()
        {
            // given

            var request = new RequestWithNoResponse();
            
            _handlerWithNoResponseMock.Setup(handler => handler.Handle(request))
                .Returns(Task.FromResult(Nothing.Value));
            
            // when
            
            var response = await _mediator.Handle(request);

            // then
            
            response.Should().Be(Nothing.Value);
            
            _handlerWithNoResponseMock.Verify(handler => handler.Handle(request), Times.Once);
        }

        [Test]
        public void mediator_fails_when_handler_of_request_is_not_registered()
        {
            // when
            
            Func<Task> handling = () => _mediator.Handle(new RequestWithResponseWithNoRegisteredHandler());
            
            // then
            
            handling.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Contain(nameof(RequestWithResponseWithNoRegisteredHandler));
        }
        
        [Test]
        public void mediator_fails_when_handler_of_request_with_no_response_is_not_registered()
        {
            // when
            
            Func<Task> handling = () => _mediator.Handle(new RequestWithNoResponseWithNoRegisteredHandler());
            
            // then

            handling.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Contain(nameof(RequestWithNoResponseWithNoRegisteredHandler));
        }

        [Test]
        public void request_cannot_be_null()
        {
            // when
            
            Func<Task> handlingRequestWithResponse = () => _mediator.Handle((RequestWithResponse) null);
            
            // then
            
            handlingRequestWithResponse.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void request_with_no_response_cannot_be_null()
        {
            // when
            
            Func<Task> handlingRequestWithNoResponse = () => _mediator.Handle((RequestWithNoResponse) null);
            
            // then
            
            handlingRequestWithNoResponse.Should().Throw<ArgumentNullException>();
        }

        public class RequestWithResponse : IRequest<Response>
        {
        }

        public class RequestWithResponseWithNoRegisteredHandler : IRequest<Response>
        {
        }

        public class Response
        {
        }

        public class RequestWithNoResponse : IRequest
        {
        }
        
        public class RequestWithNoResponseWithNoRegisteredHandler : IRequest
        {
        }
    }
}