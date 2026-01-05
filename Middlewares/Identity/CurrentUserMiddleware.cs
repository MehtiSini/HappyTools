using HappyTools.Shared.Identity;
using HappyTools.Shared.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;

namespace HappyTools.Middlewares.Identity
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity != null)
            {
                var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();

                currentUser.SetClaims(context.User);
            }

            await _next(context);
        }
    }
}