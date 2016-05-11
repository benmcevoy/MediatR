namespace MediatR.Examples.StructureMap
{
    using System;
    using System.IO;
    using global::StructureMap;

    class Program
    {
        static void Main(string[] args)
        {
            var mediator = BuildMediator();

            Runner.Run(mediator, Console.Out);

            Console.ReadKey();
        }

        private static IMediator BuildMediator()
        {
            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<PingedAsync>();
                    scanner.AssemblyContainingType<IMediator>();
                    scanner.WithDefaultConventions();
                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                });
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                cfg.For<TextWriter>().Use(Console.Out);
            });


            var mediator = container.GetInstance<IMediator>();

            return mediator;
        }
    }
}