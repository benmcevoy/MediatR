﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace MediatR.Examples.AspNetCore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var mediator = BuildMediator();
            Runner.Run(mediator, Console.Out);
            Console.ReadKey();
        }

        private static IMediator BuildMediator()
        {
            var services = new ServiceCollection();

            services.AddScoped<MultiInstanceFactory>(p => t => p.GetRequiredServices(t));

            services.AddInstance(Console.Out);

            // Use Scrutor to scan and register all
            // classes as their implemented interfaces.
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(IMediator), typeof(PingedAsync))
                .AddClasses()
                .AsImplementedInterfaces());

            var provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IMediator>();
        }

        private static IEnumerable<object> GetRequiredServices(this IServiceProvider provider, Type serviceType)
        {
            return (IEnumerable<object>) provider.GetRequiredService(typeof(IEnumerable<>).MakeGenericType(serviceType));
        } 
    }
}
