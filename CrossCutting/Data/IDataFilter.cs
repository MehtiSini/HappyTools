using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.CrossCutting.Data
{
    public interface IDataFilter<TFilter>
    {
        bool IsEnabled { get; }
        IDisposable Disable();
    }


}
