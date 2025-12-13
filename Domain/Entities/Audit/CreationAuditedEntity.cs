using HappyTools.Domain.Entities.Audit.Abstractions;

namespace HappyTools.Domain.Entities.Audit
{
    public class CreationAuditedEntity<TKey> : IEntity<TKey>, ICreationAuditedObject, IModificationAuditedObject 
    {
        public virtual TKey Id { get; set; }

        //Create
        public virtual Guid? CreatorId { get; set; }
        public virtual DateTime? CreationTime { get; set; }

        //Update
        public virtual Guid? LastModifierId { get; set; }
        public virtual DateTime? LastModificationTime { get; set; }
    }
}
