using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Shared.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace HappyTools.EfCore.Context
{
    public class MultiTenantDbContext : BaseDbContext
    {
        private readonly ICurrentTenant _currentTenant;

        protected virtual bool IsMultiTenantFilterEnabled => true;

        public MultiTenantDbContext(DbContextOptions options, ICurrentTenant currentTenant)
            : base(options)
        {
            _currentTenant = currentTenant;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            if (IsMultiTenantFilterEnabled)
                ApplyMultiTenantFilters(builder);
        }

        protected virtual void ApplyMultiTenantFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clr = entityType.ClrType;
                if (!typeof(IMultiTenant).IsAssignableFrom(clr)) continue;

                var e = Expression.Parameter(clr, "e");
                var tenantProp = Expression.Property(e, nameof(IMultiTenant.TenantId));
                var tenantValue = Expression.Constant(_currentTenant.Id, typeof(Guid?));

                var body = Expression.Equal(tenantProp, tenantValue);
                var lambda = Expression.Lambda(body, e);

                builder.Entity(clr).HasQueryFilter(lambda);
            }
        }
    }
}
