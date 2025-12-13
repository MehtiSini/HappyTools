using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Shared.MultiTenancy;

namespace HappyTools.EfCore.Interceptors
{
    public class MultiTenantInterceptor : SaveChangesInterceptor, IEfCoreInterceptor
    {
        private readonly ICurrentTenant _currentTenant;

        public MultiTenantInterceptor(ICurrentTenant currentTenant)
        {
            _currentTenant = currentTenant;
        }

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

        private void SetTenantId(DbContext? context)
        {
            if (context == null) return;

            var tenantId = _currentTenant.Id;
            if (tenantId == null) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is IMultiTenant multiTenantEntity)
                {
                    if (multiTenantEntity.TenantId == null)
                        multiTenantEntity.TenantId = tenantId;
                }
            }
        }
    }
}
