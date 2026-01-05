using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.EfCore.Uow
{
    public interface IUnitOfWorkManager
    {
        IUnitOfWork Current { get; }
        IUnitOfWork Begin();
    }

}
