using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Contract.Dtos
{
    public class EntityDto<TKey> 
    {
        public TKey Id { get; set; }
    }
}
