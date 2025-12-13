using HappyTools.Domain.Entities.Audit;
using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Domain.Entities.SoftDelete;

namespace HappyTools.Domain.BaseClasses
{
    public class MultiTenantSoftDeletedEntity<TKey> : AuditedEntity<TKey>, ISoftDelete, IMultiTenant
    {
        public bool IsDeleted { get; set; }
        public Guid? TenantId { get; set; }
    }
}
