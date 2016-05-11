namespace MediatR.Tests
{
    using Shouldly;
    using StructureMap;
    using Xunit;

    public class ExceptionTests
    {
        private readonly IMediator _mediator;

        public class Pong {}
 
        public class AsyncPinged : IAsyncNotification { }

        public ExceptionTests()
        {
            var container = new Container(cfg =>
            {
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                cfg.For<IMediator>().Use<Mediator>();
            });
            _mediator = container.GetInstance<IMediator>();
        }

        [Fact]
        public void Should_not_throw_for_async_publish()
        {
            Should.NotThrow(() => _mediator.PublishAsync(new AsyncPinged()));
        }
    }
}