using Linkdev.TeamTrack.Contract.DTOs;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using System.Linq.Expressions;

namespace Linkdev.TeamTrack.Contract.Infrastructure.Interfaces
{
    public interface IBaseRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task AddAsync(TEntity entity);
        Task AddListAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateList(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, params string[]? includes);
        Task<PaginatedResponse<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
                                                   Paging paging,
                                                   Expression<Func<TEntity, object>>? orderByKeySelector = null,
                                                   string sortingDirection = "asc",
                                                   params string[]? includes);
    }
}
