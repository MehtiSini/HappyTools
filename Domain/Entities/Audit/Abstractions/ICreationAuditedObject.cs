using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Domain.Entities.Audit.Abstractions
{
    public interface ICreationAuditedObject : IHasCreationTime, IMayHaveCreator
    {
    }
}
