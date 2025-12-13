using HappyTools.Contract.Dtos;
using HappyTools.Domain.Entities.Concurrency;
using HappyTools.Domain.Entities.MultiTenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Contract.Dtos.Identity
{
    public class IdentityUserDto : EntityDto<Guid>, IMultiTenant, IHasConcurrencyStamp
    {
        public Guid? TenantId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool IsActive { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public string ConcurrencyStamp { get; set; }

        public int EntityVersion { get; set; }

        public DateTimeOffset? LastPasswordChangeTime { get; set; }
    }
}
