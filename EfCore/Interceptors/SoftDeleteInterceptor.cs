using HappyTools.CrossCutting.Data;
using HappyTools.Domain.Entities.SoftDelete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HappyTools.EfCore.Interceptors
{
    public sealed class SoftDeleteInterceptor : SaveChangesInterceptor, IEfCoreInterceptor
    {
        private readonly IDataFilter<ISoftDelete> _filter;

        public SoftDeleteInterceptor(IDataFilter<ISoftDelete> filter)
        {
            _filter = filter;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (!_filter.IsEnabled || context == null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            var visited = new HashSet<object>();

            var deletedEntries = context.ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedEntries)
            {
                ApplyDeleteRecursively(entry, visited);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ApplyDeleteRecursively(
            EntityEntry entry,
            HashSet<object> visited)
        {
            if (visited.Contains(entry.Entity))
                return;

            visited.Add(entry.Entity);

            // Apply delete strategy
            if (entry.Entity is ISoftDelete softDelete)
            {
                softDelete.IsDeleted = true;
                entry.State = EntityState.Modified;
            }
            else
            {
                entry.State = EntityState.Deleted;
            }

            // Traverse navigation properties
            foreach (var navigation in entry.Navigations)
            {
                if (!navigation.IsLoaded)
                    continue;

                var value = navigation.CurrentValue;
                if (value == null)
                    continue;

                if (value is IEnumerable<object> collection)
                {
                    foreach (var child in collection)
                    {
                        var childEntry = entry.Context.Entry(child);
                        ApplyDeleteRecursively(childEntry, visited);
                    }
                }
                else
                {
                    var childEntry = entry.Context.Entry(value);
                    ApplyDeleteRecursively(childEntry, visited);
                }
            }
        }
    }
}
