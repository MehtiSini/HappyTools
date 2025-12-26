// BaseDbContext.cs
using HappyTools.CrossCutting.Data;
using HappyTools.Domain.Entities.SoftDelete;
using HappyTools.EfCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace HappyTools.EfCore.Context
{
    public class BaseDbContext : DbContext
    {
        protected IServiceProvider _provider;

        public BaseDbContext(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected IDataFilter<ISoftDelete> SoftDelete => _provider.GetRequiredService<IDataFilter<ISoftDelete>>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ApplyBaseConfiguration(builder);
            base.OnModelCreating(builder);
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

                var isDeletedProp =
                    Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));

                var filterEnabled =
                    Expression.Property(
                        Expression.Constant(SoftDelete),
                        nameof(IDataFilter<ISoftDelete>.IsEnabled));

                var notDeleted =
                    Expression.Equal(isDeletedProp, Expression.Constant(false));

                var body =
                    Expression.OrElse(
                        Expression.Not(filterEnabled),
                        notDeleted);

                var lambda = Expression.Lambda(body, parameter);

                builder.Entity(clr).HasQueryFilter(lambda);
            }
        }
    }
}
