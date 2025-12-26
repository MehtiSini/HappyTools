// SoftDeleteInterceptor
using HappyTools.CrossCutting.Data;
using HappyTools.Domain.Entities.SoftDelete;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace HappyTools.EfCore.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor, IEfCoreInterceptor
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
            if (!_filter.IsEnabled || eventData.Context == null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            var entries = eventData.Context.ChangeTracker.Entries()
                .Where(e => e.Entity is ISoftDelete && e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                ((ISoftDelete)entry.Entity).IsDeleted = true;
                entry.State = EntityState.Modified;
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }


    }
}