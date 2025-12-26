using Microsoft.Extensions.DependencyInjection;

namespace HappyTools.CrossCutting.Event
{
    public static class LocalEventBusServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalEventBus(this IServiceCollection services)
        {
            services.AddScoped<ILocalEventBus, LocalEventBus>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        !t.IsInterface &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(ILocalEventHandler<>)));

                foreach (var handler in handlers)
                {
                    var interfaces = handler.GetInterfaces()
                        .Where(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(ILocalEventHandler<>));

                    foreach (var @interface in interfaces)
                    {
                        services.AddScoped(@interface, handler);
                    }
                }
            }

            return services;
        }
    }
}