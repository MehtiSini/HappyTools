using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.CrossCutting.Data
{
    // DataFilter implementation
    public sealed class DataFilter<TFilter> : IDataFilter<TFilter>
    {
        private static readonly AsyncLocal<int> _disableCount = new();

        public bool IsEnabled => _disableCount.Value == 0;

        public IDisposable Disable()
        {
            _disableCount.Value++;
            return new Revert(() => _disableCount.Value--);
        }

        public IDisposable Enable()
        {
            if (_disableCount.Value > 0)
                _disableCount.Value--;
            return new Revert(() => _disableCount.Value++);
        }

        private sealed class Revert : IDisposable
        {
            private readonly Action _onDispose;
            private bool _disposed;

            public Revert(Action onDispose) => _onDispose = onDispose;

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _onDispose();
            }
        }
    }

}
