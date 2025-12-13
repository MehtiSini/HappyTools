using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HappyTools.Contract.Dtos.Identity
{
    public class IdentityUserCreateDto : IdentityUserCreateOrUpdateDtoBase
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string RePassword { get; set; }

        public string NationalCode { get; set; }

    }
}
