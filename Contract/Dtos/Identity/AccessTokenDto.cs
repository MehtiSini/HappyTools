using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Contract.Dtos.Identity
{
    public class AccessTokenDto
    {
        public string AccessToken { get; set; }
        public int ExpireIn { get; set; }
    }
}
