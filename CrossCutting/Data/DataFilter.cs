using HappyTools.DependencyInjection.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.CrossCutting.Data
{
    public class DataFilter<TFilter> : IDataFilter<TFilter>
    {
        private static readonly AsyncLocal<int> _disableCount = new();

        public bool IsEnabled => _disableCount.Value == 0;

        public IDisposable Disable()
        {
            _disableCount.Value++;
            return new ReEnableScope();
        }

        private sealed class ReEnableScope : IDisposable
        {
            public void Dispose()
            {
                _disableCount.Value--;
            }
        }
    }

}
