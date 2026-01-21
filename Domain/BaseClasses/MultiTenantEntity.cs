using HappyTools.Domain.Entities.Audit;
using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Domain.Entities.SoftDelete;

namespace HappyTools.Domain.BaseClasses
{
    public class MultiTenantEntity<TKey> : AuditedEntity<TKey>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
    }
}
