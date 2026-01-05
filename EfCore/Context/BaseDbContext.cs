// BaseDbContext.cs
using HappyTools.CrossCutting.Data;
using HappyTools.Domain.Entities.SoftDelete;
using HappyTools.EfCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace HappyTools.EfCore.Context
{
    public class BaseDbContext<TContext> : DbContext where TContext : DbContext
    {
        protected IServiceProvider _provider;

        public BaseDbContext(DbContextOptions<TContext> options, IServiceProvider provider)
        : base(options)
        {
            _provider = provider;
        }

        protected IDataFilter<ISoftDelete> SoftDelete => _provider.GetRequiredService<IDataFilter<ISoftDelete>>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ApplyBaseConfiguration(builder);
        }

        protected virtual void ApplyBaseConfiguration(ModelBuilder builder)
        {
            builder.ApplyIEntityPrimaryKeys();
            ApplySoftDeleteFilters(builder);
        }

        protected virtual void ApplySoftDeleteFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clr = entityType.ClrType;
                if (!typeof(ISoftDelete).IsAssignableFrom(clr))
                    continue;

                var parameter = Expression.Parameter(clr, "e");

                // e.IsDeleted
                var isDeletedProp = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));

                // SoftDelete.IsEnabled (evaluated at runtime)
                var softDeleteProperty = Expression.Property(
                    Expression.Constant(SoftDelete),
                    nameof(IDataFilter<ISoftDelete>.IsEnabled)
                );

                // !SoftDelete.IsEnabled
                var notEnabled = Expression.Not(softDeleteProperty);

                // e.IsDeleted == false
                var notDeleted = Expression.Equal(isDeletedProp, Expression.Constant(false));

                // Combine: !SoftDelete.IsEnabled || e.IsDeleted == false
                var filterBody = Expression.OrElse(notEnabled, notDeleted);

                var lambda = Expression.Lambda(filterBody, parameter);

                builder.Entity(clr).HasQueryFilter(lambda);
            }
        }

    }
}
