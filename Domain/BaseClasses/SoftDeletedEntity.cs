using HappyTools.Domain.Entities.Audit;
using HappyTools.Domain.Entities.SoftDelete;

namespace HappyTools.Domain.BaseClasses
{
    public class SoftDeletedEntity<TKey> : AuditedEntity<TKey>, ISoftDelete
    {
        public virtual bool IsDeleted { get; set; }
    }

}
