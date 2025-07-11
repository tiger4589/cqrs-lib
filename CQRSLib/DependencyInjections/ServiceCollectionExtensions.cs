﻿using System.Reflection;
using CQRSLib.Commands;
using CQRSLib.Core;
using CQRSLib.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSLib.DependencyInjections;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandQueryLibrary(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddTransient<IDispatcher, Dispatcher>();

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
