namespace MediatR.Examples.SimpleInjector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::SimpleInjector;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var mediator = BuildMediator();

            Runner.Run(mediator, Console.Out);

            Console.ReadKey();
        }

        private static IMediator BuildMediator()
        {
            var container = new Container();
            var assemblies = GetAssemblies().ToArray();
            container.RegisterSingleton<IMediator, Mediator>();
            container.RegisterCollection(typeof(IAsyncNotificationHandler<>), assemblies);
            container.RegisterSingleton(Console.Out);
            container.RegisterSingleton(new MultiInstanceFactory(container.GetAllInstances));

            container.Verify();

            var mediator = container.GetInstance<IMediator>();

            return mediator;
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
        }
    }
}
