using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace HappyTools.Domain.Entities.Audit.Abstractions
{

    public interface IModificationAuditedObject : IHasModificationTime
    {
        Guid? LastModifierId { get; set; }
    }

}
