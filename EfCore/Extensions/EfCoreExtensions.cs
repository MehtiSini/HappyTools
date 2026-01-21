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

            services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();

            services.AddScoped<MultiTenantInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<AuditingInterceptor>();

            services.AddDbContext<TContext>((provider, options) =>
            {
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

                options.AddInterceptors(provider.GetRequiredService<MultiTenantInterceptor>());
                options.AddInterceptors(provider.GetRequiredService<SoftDeleteInterceptor>());
                options.AddInterceptors(provider.GetRequiredService<AuditingInterceptor>());

            });

            return services;
        }
    }
}
