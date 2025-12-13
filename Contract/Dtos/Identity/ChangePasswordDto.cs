using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HappyTools.Contract.Dtos.Identity
{
    public class ChangePasswordDto
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string RePassword { get; set; }
    }
}
