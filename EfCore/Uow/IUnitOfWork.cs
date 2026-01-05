using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.EfCore.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext;
        Task CompleteAsync();
    }



}
