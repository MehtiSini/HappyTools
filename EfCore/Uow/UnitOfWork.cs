using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HappyTools.DependencyInjection.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTools.EfCore.Uow
{
    public class UnitOfWork : IUnitOfWork 
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, DbContext> _dbContexts = new();
        private bool _completed;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {
            var type = typeof(TDbContext);
            if (_dbContexts.TryGetValue(type, out var existing))
                return (TDbContext)existing;

            var dbContext = _serviceProvider.GetRequiredService<TDbContext>();
            _dbContexts[type] = dbContext;
            return dbContext;
        }

        public async Task CompleteAsync()
        {
            if (_completed) return;

            foreach (var dbContext in _dbContexts.Values)
                await dbContext.SaveChangesAsync();

            _completed = true;
        }

        public void Dispose()
        {
            foreach (var dbContext in _dbContexts.Values)
                dbContext.Dispose();
        }
    }


}
