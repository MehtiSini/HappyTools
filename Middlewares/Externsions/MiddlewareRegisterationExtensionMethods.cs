using HappyTools.Middlewares.Identity;
using HappyTools.Middlewares.Multitenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTools.Middlewares.Externsions
{
    public static class MiddlewareRegisterationExtensionMethods
    {
        public static IApplicationBuilder UseCurrentUserMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CurrentUserMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseCurrentTenant(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CurrentTenantMiddleware>();
        }

    }
}
