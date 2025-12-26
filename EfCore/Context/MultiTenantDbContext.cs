using HappyTools.CrossCutting.Data;
using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Domain.Entities.SoftDelete;
using HappyTools.EfCore.Extensions;
using HappyTools.Shared.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;

namespace HappyTools.EfCore.Context
{
    public class MultiTenantDbContext : BaseDbContext
    {
        public MultiTenantDbContext(DbContextOptions options, IServiceProvider provider) : base(options, provider)
        {
        }

        protected ICurrentTenant CurrentTenant => _provider.GetRequiredService<ICurrentTenant>();

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
                var tenantValue = Expression.Constant(CurrentTenant.Id, typeof(Guid?));

                var body = Expression.Equal(tenantProp, tenantValue);
                var lambda = Expression.Lambda(body, e);

                builder.Entity(clr).HasQueryFilter(lambda);
            }
        }
    }
}
