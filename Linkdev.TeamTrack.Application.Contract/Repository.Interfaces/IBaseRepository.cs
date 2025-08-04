using Linkdev.TeamTrack.Core.Models;
using System.Linq.Expressions;

namespace Linkdev.TeamTrack.Contract.Repository.Interfaces
{
    public interface IBaseRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    }
}
