using HappyTools.DependencyInjection.Contracts;
using System;

namespace HappyTools.Shared.MultiTenancy
{
    public class CurrentTenant : ICurrentTenant, IScopedDependency
    {
        private Guid? _id;
        private string? _name;

        public bool IsAvailable => _id.HasValue;

        public Guid? Id => _id;

        public string? Name => _name;

        public IDisposable Change(Guid? id, string? name = null)
        {
            var previousId = _id;
            var previousName = _name;

            _id = id;
            _name = name;

            return new TenantRestore(previousId, previousName, this);
        }

        private class TenantRestore : IDisposable
        {
            private readonly Guid? _previousId;
            private readonly string? _previousName;
            private readonly CurrentTenant _currentTenant;
            private bool _disposed;

            public TenantRestore(Guid? previousId, string? previousName, CurrentTenant currentTenant)
            {
                _previousId = previousId;
                _previousName = previousName;
                _currentTenant = currentTenant;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _currentTenant._id = _previousId;
                    _currentTenant._name = _previousName;
                    _disposed = true;
                }
            }
        }
    }
}
