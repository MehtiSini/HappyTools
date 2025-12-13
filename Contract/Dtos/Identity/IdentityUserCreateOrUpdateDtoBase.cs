using HappyTools.Domain.Entities.MultiTenant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HappyTools.Contract.Dtos.Identity
{
    public abstract class IdentityUserCreateOrUpdateDtoBase : IMultiTenant
    {
        [Required]
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

       
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [JsonIgnore]
        public string RoleName { get; set; }

        [JsonIgnore]
        public bool IsActive { get; set; } = true;
        [JsonIgnore]

        public bool LockoutEnabled { get; set; }

        [JsonIgnore]
        public Guid? TenantId { get; set; }


    }
}
