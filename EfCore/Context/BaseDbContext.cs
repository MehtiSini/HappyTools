// BaseDbContext.cs
using HappyTools.Domain.Entities.SoftDelete;
using HappyTools.EfCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HappyTools.EfCore.Context
{
    public class BaseDbContext : DbContext
    {
        protected virtual bool IsSoftDeleteFilterEnabled => true;

        public BaseDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ApplyBaseConfiguration(builder);
            base.OnModelCreating(builder);
        }

        protected virtual void ApplyBaseConfiguration(ModelBuilder builder)
        {
            builder.ApplyIEntityPrimaryKeys();

            if (IsSoftDeleteFilterEnabled)
                ApplySoftDeleteFilters(builder);
        }

        protected virtual void ApplySoftDeleteFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clr = entityType.ClrType;
                if (!typeof(ISoftDelete).IsAssignableFrom(clr)) continue;

                var e = Expression.Parameter(clr, "e");
                var isDeletedProp = Expression.Property(e, nameof(ISoftDelete.IsDeleted));
                var body = Expression.Equal(isDeletedProp, Expression.Constant(false));
                var lambda = Expression.Lambda(body, e);

                builder.Entity(clr).HasQueryFilter(lambda);
            }
        }
    }
}
