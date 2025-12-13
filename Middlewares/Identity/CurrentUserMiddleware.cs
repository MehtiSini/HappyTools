using HappyTools.Shared.Identity;
using Microsoft.AspNetCore.Http;
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

        public async Task InvokeAsync(HttpContext context, CurrentUser currentUser)
        {
            if (context.User?.Identity != null)
            {
                currentUser.SetClaims(context.User);
            }

            await _next(context);
        }
    }
}