using HappyTools.Shared.Identity;
using HappyTools.Shared.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace HappyTools.Middlewares.Multitenancy
{
    public class CurrentTenantMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentTenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentTenant currentTenant, ICurrentUser currentUser)
        {
            if (currentUser.IsAuthenticated && currentUser.TenantId.HasValue)
            {
                currentTenant.Change(currentUser?.TenantId.Value, null);
            }

            await _next(context);
        }
    }
}