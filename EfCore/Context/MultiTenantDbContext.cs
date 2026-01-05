using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.EfCore.Extensions;
using HappyTools.Shared.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace HappyTools.EfCore.Context
{
    public class MultiTenantDbContext<TContext> : BaseDbContext<TContext>
        where TContext : BaseDbContext<TContext>
    {
        public MultiTenantDbContext(DbContextOptions<TContext> options, IServiceProvider provider)
            : base(options, provider)
        {
        }

        protected ICurrentTenant CurrentTenant => _provider.GetRequiredService<ICurrentTenant>();

        protected override void ApplyBaseConfiguration(ModelBuilder builder)
        {
            base.ApplyBaseConfiguration(builder);

            ApplyMultiTenantFilters(builder);
            ApplySoftDeleteFilters(builder);
        }

        protected virtual void ApplyMultiTenantFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clr = entityType.ClrType;

                if (!typeof(IMultiTenant).IsAssignableFrom(clr))
                    continue;

                var parameter = Expression.Parameter(clr, "e");
                var tenantIdProp = Expression.Property(parameter, nameof(IMultiTenant.TenantId));

                // Lazy access to CurrentTenant
                var currentTenantExpr = Expression.Property(
                    Expression.Constant(this),
                    nameof(CurrentTenant));

                var tenantIdExpr = Expression.Property(currentTenantExpr, nameof(ICurrentTenant.Id));

                // Filter: e.TenantId == CurrentTenant.Id
                var filterBody = Expression.Equal(tenantIdProp, tenantIdExpr);

                var lambda = Expression.Lambda(filterBody, parameter);

                builder.Entity(clr).HasQueryFilter(lambda);
            }
        }

    }
}