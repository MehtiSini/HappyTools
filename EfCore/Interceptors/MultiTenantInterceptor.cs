using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Shared.MultiTenancy;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HappyTools.EfCore.Interceptors
{
    public class MultiTenantInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            SetTenantId(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            SetTenantId(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void SetTenantId(DbContext? context)
        {
            if (context == null) return;

            var currentTenant = context.GetService<ICurrentTenant>();
            var tenantId = currentTenant?.Id;
            if (tenantId == null) return;

            foreach (var entry in context.ChangeTracker.Entries<IMultiTenant>())
            {
                if (entry.Entity.TenantId == null)
                    entry.Entity.TenantId = tenantId;
            }
        }
    }
}