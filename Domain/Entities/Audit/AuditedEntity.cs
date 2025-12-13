using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HappyTools.Domain.Entities.Concurrency;
using MassTransit;

namespace HappyTools.Domain.Entities.Audit
{
    public class AuditedEntity<TKey> : CreationAuditedEntity<TKey>,  IHasConcurrencyStamp
    {
        public AuditedEntity()
        {
            if (typeof(TKey) == typeof(Guid))
            {
                Id = (TKey)(object)NewId.Next().ToGuid();
            }

            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        public virtual string ConcurrencyStamp { get; set; }
    }
}
