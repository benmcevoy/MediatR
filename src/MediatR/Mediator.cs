using MediatR.Internal;

namespace MediatR
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Default mediator implementation relying on single- and multi instance delegates for resolving handlers.
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly MultiInstanceFactory _multiInstanceFactory;

        private readonly ConcurrentDictionary<Type, Type> _genericHandlerCache;
        private readonly ConcurrentDictionary<Type, Type> _wrapperHandlerCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="multiInstanceFactory">The multi instance factory.</param>
        public Mediator(MultiInstanceFactory multiInstanceFactory)
        {
            _multiInstanceFactory = multiInstanceFactory;
            _genericHandlerCache = new ConcurrentDictionary<Type, Type>();
            _wrapperHandlerCache = new ConcurrentDictionary<Type, Type>();
        }
        
        public Task PublishAsync(IAsyncNotification notification)
        {
            var notificationHandlers = GetNotificationHandlers(notification)
                .Select(handler => handler.Handle(notification))
                .ToArray();

            return Task.WhenAll(notificationHandlers);
        }

        public Task PublishAsync(ICancellableAsyncNotification notification, CancellationToken cancellationToken)
        {
            var notificationHandlers = GetNotificationHandlers(notification)
                .Select(handler => handler.Handle(notification, cancellationToken))
                .ToArray();

            return Task.WhenAll(notificationHandlers);
        }

        private IEnumerable<AsyncNotificationHandlerWrapper> GetNotificationHandlers(IAsyncNotification notification)
        {
            return GetNotificationHandlers<AsyncNotificationHandlerWrapper>(notification,
                typeof(IAsyncNotificationHandler<>),
                typeof(AsyncNotificationHandlerWrapper<>));
        }

        private IEnumerable<CancellableAsyncNotificationHandlerWrapper> GetNotificationHandlers(ICancellableAsyncNotification notification)
        {
            return GetNotificationHandlers<CancellableAsyncNotificationHandlerWrapper>(notification,
                typeof (ICancellableAsyncNotificationHandler<>),
                typeof(CancellableAsyncNotificationHandlerWrapper<>));
        }

        private IEnumerable<TWrapper> GetNotificationHandlers<TWrapper>(object notification, Type handlerType, Type wrapperType)
        {
            var notificationType = notification.GetType();

            var genericHandlerType = _genericHandlerCache.GetOrAdd(notificationType, handlerType, (type, root) => root.MakeGenericType(type));
            var genericWrapperType = _wrapperHandlerCache.GetOrAdd(notificationType, wrapperType, (type, root) => root.MakeGenericType(type));

            return GetNotificationHandlers(notification, genericHandlerType)
                .Select(handler => Activator.CreateInstance(genericWrapperType, handler))
                .Cast<TWrapper>()
                .ToList();
        }

        private IEnumerable<object> GetNotificationHandlers(object notification, Type handlerType)
        {
            try
            {
                return _multiInstanceFactory(handlerType);
            }
            catch (Exception e)
            {
                throw BuildException(notification, e);
            }
        }

        private static InvalidOperationException BuildException(object message, Exception inner)
        {
            return new InvalidOperationException("Handler was not found for request of type " + message.GetType() + ".\r\nContainer or service locator not configured properly or handlers not registered with your container.", inner);
        }
    }
}
