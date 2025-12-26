using HappyTools.CrossCutting.Data;
using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Domain.Entities.SoftDelete;
using HappyTools.Shared.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace HappyTools.EfCore.Context
{
    public class MultiTenantDbContext : BaseDbContext
    {
        private readonly ICurrentTenant _currentTenant;

        public MultiTenantDbContext(DbContextOptions options, IDataFilter<ISoftDelete> softDeleteFilter) : base(options, softDeleteFilter)
        {
        }

        protected virtual bool IsMultiTenantFilterEnabled => true;

      
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
