using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace HappyTools.CrossCutting.Wrapper.AutoWrapper
{

    /// <summary>
    /// Extension methods for the AutoWrapper middleware.
    /// </summary>
    public static class AutoWrapperExtension
    {
        public static IApplicationBuilder UseApiResponseAndExceptionWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AutoWrapperMiddleware>();
        }

        //public static IApplicationBuilder UseApiResponseAndExceptionWrapper<T>(this IApplicationBuilder builder)
        //{
        //    options ??= new AutoWrapperOptions();
        //    return builder.UseMiddleware<AutoWrapperMiddleware<T>>(options);
        //}


        /// <summary>
        /// Add auto response wrapper services using default settings
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAutoWrapperMiddleware(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }


            services.Configure<AutoWrapperOptions>(opts => configurationSection.Bind(opts));
            return services;
        }

        /// <summary>
        /// Add auto response wrapper services and configure the related options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="AutoWrapperOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAutoWrapperMiddleware(this IServiceCollection services, Action<AutoWrapperOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);
            //services.TryAddSingleton<IResponseCompressionProvider, ResponseCompressionProvider>();
            return services;
        }
    }

}
