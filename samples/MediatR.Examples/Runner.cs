namespace MediatR.Examples
{
    using System.IO;
    using System.Threading.Tasks;

    public static class Runner
    {
        public static void Run(IMediator mediator, TextWriter writer)
        {
            writer.WriteLine("Sample mediator implementation using send, publish and post-request handlers in sync and async version.");
            writer.WriteLine("---------------");
            
            writer.WriteLine("Publishing Pinged async...");
            var publishResponse = mediator.PublishAsync(new PingedAsync());
            Task.WaitAll(publishResponse);
        }
    }
}