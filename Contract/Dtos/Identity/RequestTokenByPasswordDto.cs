using System;
using System.ComponentModel.DataAnnotations;

namespace HappyTools.Contract.Dtos.Identity
{
    public class RequestTokenByPasswordDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}