using HappyTools.Domain.Entities.Concurrency;
using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Domain.Entities.SoftDelete;

namespace HappyTools.Domain.Entities.Audit.Abstractions
{
    // Base audited entity
    public interface IAuditedEntity<TKey>
        : IEntity<TKey>, ICreationAuditedObject, IModificationAuditedObject
    {
    }

    // Audited entity with concurrency support
    public interface IAuditedConcurrentEntity<TKey>
        : IAuditedEntity<TKey>, IHasConcurrencyStamp
    {
    }

    // Audited entity with concurrency + soft delete
    public interface ISoftDeletedAuditedConcurrentEntity<TKey>
        : IAuditedConcurrentEntity<TKey>, ISoftDelete
    {
    }

    // Audited entity with concurrency + soft delete + multi-tenant
    public interface IMultiTenantSoftDeletedAuditedEntity<TKey>
        : ISoftDeletedAuditedConcurrentEntity<TKey>, IMultiTenant
    {
    }

    // Audited entity with soft delete + multi-tenant (no concurrency)
    public interface ISoftDeletedMultiTenantEntity<TKey>
        : IAuditedEntity<TKey>, ISoftDelete, IMultiTenant
    {
    }
}
