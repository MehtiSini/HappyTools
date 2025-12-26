using HappyTools.Domain.Entities.Audit.Abstractions;
using HappyTools.Shared;
using System.Linq.Expressions;

namespace HappyTools.Repository
{
    public interface IRepository<TEntity, TKey>
      where TEntity : IEntity<TKey>
    {
        Task<TKey> InsertAsync(TEntity entity);
        Task<TKey> UpdateAsync(TEntity entity);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TKey>> GetListAsync();
    }

    public interface IRepository<TEntity, TKey, TFilterModel>
        where TEntity : IEntity<TKey>
       where TFilterModel : BaseFilterModel
    {
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TKey>> GetListAsync();
        Task<IReadOnlyList<TKey>> GetFilteredListAsync(TFilterModel filterModel);
    }
}
