using HappyTools.DependencyInjection.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace HappyTools.DependencyInjection.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddConventionalServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract &&
                            (typeof(ITransientDependency).IsAssignableFrom(t) ||
                             typeof(IScopedDependency).IsAssignableFrom(t) ||
                             typeof(ISingletonDependency).IsAssignableFrom(t)))
                .ToList();



            foreach (var type in types)
            {
                var serviceInterfaces = type.GetInterfaces()
                    .Where(i => i != typeof(ITransientDependency)
                             && i != typeof(IScopedDependency)
                             && i != typeof(ISingletonDependency))
                    .ToList();

                if (typeof(ITransientDependency).IsAssignableFrom(type))
                {
                    foreach (var iface in serviceInterfaces)
                        services.AddTransient(iface, type);
                }
                else if (typeof(IScopedDependency).IsAssignableFrom(type))
                {
                    foreach (var iface in serviceInterfaces)
                        services.AddScoped(iface, type);
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(type))
                {
                    foreach (var iface in serviceInterfaces)
                        services.AddSingleton(iface, type);
                }
            }

            return services;
        }
    }
}
