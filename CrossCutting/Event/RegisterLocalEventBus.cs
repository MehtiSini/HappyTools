using Microsoft.Extensions.DependencyInjection;

namespace HappyTools.CrossCutting.Event
{
    public static class LocalEventBusServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalEventBus(this IServiceCollection services)
        {
            // Event bus itself
            services.AddScoped<ILocalEventBus, LocalEventBus>();

            // Scan & register all handlers
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        !t.IsInterface &&
                        t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(ILocalEventHandler<>)));

                foreach (var handlerType in handlerTypes)
                {
                    var interfaces = handlerType.GetInterfaces()
                        .Where(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(ILocalEventHandler<>));

                    foreach (var @interface in interfaces)
                    {
                        services.AddScoped(@interface, handlerType);
                    }
                }
            }

            return services;
        }
    }

}