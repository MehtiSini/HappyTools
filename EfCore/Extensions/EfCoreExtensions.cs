using HappyTools.CrossCutting.Data;
using HappyTools.EfCore.Interceptors;
using HappyTools.EfCore.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace HappyTools.EfCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDbContext<TContext>(
            this IServiceCollection services,
            string connectionString,
            string providerName
        ) where TContext : DbContext
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("ConnectionString is mandatory!");

            services.AddScoped(typeof(IDataFilter<>), typeof(DataFilter<>));

            // Register EF Core interceptor(s)
            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<IInterceptor>(sp => sp.GetRequiredService<SoftDeleteInterceptor>());

            services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();

            services.AddDbContext<TContext>((provider, options) =>
            {
                // Database provider
                switch (providerName)
                {
                    case DbProvider.SqlServer:
                        options.UseSqlServer(connectionString);
                        break;
                    case DbProvider.PostgreSQL:
                        options.UseNpgsql(connectionString);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported provider: {providerName}");
                }

                // Add all registered interceptors
                var interceptors = provider.GetServices<IInterceptor>().ToArray();
                if (interceptors.Any())
                    options.AddInterceptors(interceptors);
            });

            return services;
        }
    }
}
