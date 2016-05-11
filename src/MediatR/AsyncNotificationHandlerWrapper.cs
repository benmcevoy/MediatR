using System.Threading;
using System.Threading.Tasks;

namespace MediatR
{
    internal abstract class AsyncNotificationHandlerWrapper
    {
        public abstract Task Handle(IAsyncNotification message);
    }

    internal class AsyncNotificationHandlerWrapper<TNotification> : AsyncNotificationHandlerWrapper
        where TNotification : IAsyncNotification
    {
        private readonly IAsyncNotificationHandler<TNotification> _inner;

        public AsyncNotificationHandlerWrapper(IAsyncNotificationHandler<TNotification> inner)
        {
            _inner = inner;
        }

        public override Task Handle(IAsyncNotification message)
        {
            return _inner.Handle((TNotification)message);
        }
    }

    internal abstract class CancellableAsyncNotificationHandlerWrapper
    {
        public abstract Task Handle(ICancellableAsyncNotification message, CancellationToken cancellationToken);
    }

    internal class CancellableAsyncNotificationHandlerWrapper<TNotification> : CancellableAsyncNotificationHandlerWrapper
        where TNotification : ICancellableAsyncNotification
    {
        private readonly ICancellableAsyncNotificationHandler<TNotification> _inner;

        public CancellableAsyncNotificationHandlerWrapper(ICancellableAsyncNotificationHandler<TNotification> inner)
        {
            _inner = inner;
        }

        public override Task Handle(ICancellableAsyncNotification message, CancellationToken cancellationToken)
        {
            return _inner.Handle((TNotification)message, cancellationToken);
        }
    }
}