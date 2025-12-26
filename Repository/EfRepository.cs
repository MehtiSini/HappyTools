using HappyTools.Domain.Entities.Audit.Abstractions;
using HappyTools.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HappyTools.Repository
{
    public class EfRepository<TDbContext, TEntity, TKey>
       : IRepository<TEntity, TKey>
       where TDbContext : DbContext
       where TEntity : class, IEntity<TKey>
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> _set;

        public EfRepository(TDbContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }

        public virtual async Task<TKey> InsertAsync(TEntity entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public virtual async Task<TKey> UpdateAsync(TEntity entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public virtual async Task<TKey> DeleteAsync(TKey id)
        {
            var entity = await _set.FirstOrDefaultAsync(x => x.Id!.Equals(id));
            if (entity == null)
                throw new KeyNotFoundException();

            _set.Remove(entity);
            await _context.SaveChangesAsync();
            return id;
        }

        public virtual async Task<IReadOnlyList<TKey>> GetListAsync()
        {
            return await _set
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();
        }

        public virtual async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return await Task.FromResult(_set.AsNoTracking().AsQueryable());
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await _set.FirstOrDefaultAsync(predicate);
            if (entity == null)
                throw new KeyNotFoundException();

            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public class EfRepository<TDbContext, TEntity, TKey, TFilterModel>
        : IRepository<TEntity, TKey, TFilterModel>
        where TDbContext : DbContext
        where TEntity : class, IEntity<TKey>
        where TFilterModel : BaseFilterModel
    {
        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> _set;

        public EfRepository(TDbContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TKey> DeleteAsync(TKey id)
        {
            var entity = await _set.FirstOrDefaultAsync(x => x.Id!.Equals(id));
            if (entity == null)
                throw new KeyNotFoundException();

            _set.Remove(entity);
            await _context.SaveChangesAsync();
            return id;
        }

        public virtual async Task<IReadOnlyList<TKey>> GetListAsync()
        {
            return await _set
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();
        }

        public virtual async Task<IReadOnlyList<TKey>> GetFilteredListAsync(TFilterModel filterModel)
        {
            IQueryable<TEntity> query = _set.AsNoTracking();


            return await query
                .Select(x => x.Id)
                .ToListAsync();
        }

        public virtual async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return await Task.FromResult(_set.AsNoTracking().AsQueryable());
        }

        public virtual async Task<IQueryable<TEntity>> GetFilteredQueryAsync(TFilterModel filterModel)
        {
            return await GetQueryableAsync();
        }



        public virtual async Task InsertManyAsync(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await InsertAsync(entity);
            }

        }

        public virtual async Task UpdateManyAsync(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await UpdateAsync(entity);
            }

        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await _set.FirstOrDefaultAsync(predicate);
            if (entity == null)
                throw new KeyNotFoundException();

            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
