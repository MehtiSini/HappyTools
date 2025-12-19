using HappyTools.Domain.Entities.Audit.Abstractions;
using HappyTools.Domain.Entities.Concurrency;
using HappyTools.Domain.Entities.MultiTenant;
using HappyTools.Domain.Entities.SoftDelete;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Identity
{
    public class BaseMultiTenantIdentityUser<TKey> : IdentityUser<Guid>, ISoftDeletedMultiTenantEntity<Guid>
    {
        public bool IsDeleted { get; set; }

        public DateTime? CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public Guid? TenantId { get; set; }

        
    }

    public class BaseTenantIdentityRole : IdentityRole<Guid>, ISoftDeletedAuditedConcurrentEntity<Guid>
    {
        public bool IsDeleted { get; set; }

        public DateTime? CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }

    public class BaseTenantIdentityUserClaim<TKey> : IdentityUserClaim<Guid>, ISoftDeletedMultiTenantEntity<Guid>
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }
        public Guid? TenantId { get; set; }
    }

    public class BaseMultiTenantIdentityUserRole<TKey> : IdentityUserRole<Guid>, ISoftDeletedMultiTenantEntity<Guid>
    {
        [Required]
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? TenantId { get; set; }
       
    }


    public class BaseMultiTenantIdentityUserLogin<TKey> : IdentityUserLogin<Guid>, IMultiTenantSoftDeletedAuditedEntity<Guid>
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? TenantId { get; set; }
        public string? ConcurrencyStamp { get; set; }
    }

    public class BaseMultiTenantIdentityUserToken<TKey> : IdentityUserToken<Guid>, IMultiTenantSoftDeletedAuditedEntity<Guid>
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? TenantId { get; set; }
        public string? ConcurrencyStamp { get; set; }
    }

    public class BaseMultiTenantIdentityRoleClaim<TKey> : IdentityRoleClaim<Guid>, IMultiTenantSoftDeletedAuditedEntity<Guid>
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? TenantId { get; set; }
        public string? ConcurrencyStamp { get; set; }
    }
}
