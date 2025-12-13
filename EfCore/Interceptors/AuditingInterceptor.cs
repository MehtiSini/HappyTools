using System;
using System.Threading;
using System.Threading.Tasks;
using HappyTools.Domain.Entities.Audit.Abstractions;
using HappyTools.Shared.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HappyTools.EfCore.Interceptors
{
    public class AuditingInterceptor : SaveChangesInterceptor , IEfCoreInterceptor
    {
        private readonly ICurrentUser _currentUser;

        public AuditingInterceptor(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateAuditProperties(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditProperties(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditProperties(DbContext? context)
        {
            if (context == null) return;

            var now = DateTime.UtcNow;
            var userId = _currentUser?.Id;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is ICreationAuditedObject creationAudited)
                {
                    if (entry.State == EntityState.Added)
                    {
                        creationAudited.CreationTime = now;
                        if (creationAudited is IMayHaveCreator mayHaveCreator && userId.HasValue)
                        {
                            mayHaveCreator.CreatorId = userId;
                        }
                    }
                }

                if (entry.Entity is IModificationAuditedObject modificationAudited)
                {
                    if (entry.State == EntityState.Modified)
                    {
                        modificationAudited.LastModificationTime = now;
                        if (userId.HasValue)
                        {
                            modificationAudited.LastModifierId = userId;
                        }
                    }
                }

            }
        }
    }
}