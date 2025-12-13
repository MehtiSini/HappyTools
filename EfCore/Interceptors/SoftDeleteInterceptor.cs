using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HappyTools.Domain.Entities.SoftDelete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HappyTools.EfCore.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor, IEfCoreInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context;
            if (context == null) return base.SavingChanges(eventData, result);

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is ISoftDelete && e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                ((ISoftDelete)entry.Entity).IsDeleted = true;
                entry.State = EntityState.Modified;
            }

            return base.SavingChanges(eventData, result);
        }

    }
}
