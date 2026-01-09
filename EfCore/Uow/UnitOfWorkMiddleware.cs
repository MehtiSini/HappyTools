using HappyTools.Shared.Identity;
using HappyTools.Shared.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTools.EfCore.Uow
{
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;
        public UnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var _uowManager = context.RequestServices.GetRequiredService<IUnitOfWorkManager>();

            var c = context.RequestServices.GetRequiredService<ICurrentTenant>();

            using var uow = _uowManager.Begin();
            await _next(context);
            await uow.CompleteAsync();
        }
    }
}

