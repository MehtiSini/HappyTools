using HappyTools.DependencyInjection.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.EfCore.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager  , IScopedDependency
    {
        private static readonly AsyncLocal<IUnitOfWork> _current = new();

        private readonly IServiceProvider _serviceProvider;
        public IUnitOfWork Current => _current.Value;

        public UnitOfWorkManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork Begin()
        {
            var uow = new UnitOfWork(_serviceProvider);
            _current.Value = uow;
            return uow;
        }
    }

}
