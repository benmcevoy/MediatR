using System.Threading;
using System.Threading.Tasks;

namespace MediatR
{
    /// <summary>
    /// Defines a mediator to encapsulate request/response and publishing interaction patterns
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Asynchronously send a notification to multiple handlers
        /// </summary>
        /// <param name="notification">Notification object</param>
        /// <returns>A task that represents the publish operation.</returns>
        Task PublishAsync(IAsyncNotification notification);

        /// <summary>
        /// Asynchronously send a cancellable notification to multiple handlers
        /// </summary>
        /// <param name="notification">Notification object</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the publish operation.</returns>
        Task PublishAsync(ICancellableAsyncNotification notification, CancellationToken cancellationToken);
    }
}