using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
                throw new Exception("ConnectionString Is Mandatory!");

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

                var interceptors = provider.GetServices<IInterceptor>().ToArray();
                if (interceptors.Any())
                    options.AddInterceptors(interceptors);
            });

            return services;
        }
    }
}
