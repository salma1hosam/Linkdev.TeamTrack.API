using Linkdev.TeamTrack.Contract.DTOs;
using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Linkdev.TeamTrack.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Linkdev.TeamTrack.Infrastructure.Repositories
{
    public class BaseRepository<TEntity, TKey>(TeamTrackDbContext _dbContext) : IBaseRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);

        public async Task AddListAsync(List<TEntity> entities) => await _dbContext.Set<TEntity>().AddRangeAsync(entities);

        public void Remove(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

        public void UpdateList(List<TEntity> entities) => _dbContext.Set<TEntity>().UpdateRange(entities);

        public async Task<TEntity?> GetByIdAsync(TKey id) => await _dbContext.Set<TEntity>().FindAsync(id);

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, params string[]? includes)
        {
            var items = _dbContext.Set<TEntity>().Where(predicate);
            if (includes?.Any() == true)
                foreach (var include in includes)
                    items = items.Include(include);

            return items;
        }

        public async Task<PaginatedResponse<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
                                                                Paging paging,
                                                                Expression<Func<TEntity, object>>? orderByKeySelector = null,
                                                                string sortingDirection = "asc",
                                                                params string[]? includes)
        {

            var query = _dbContext.Set<TEntity>().Where(predicate);

            if (orderByKeySelector is not null)
            {
                if (sortingDirection.ToLower().Contains("desc"))
                    query = query.OrderByDescending(orderByKeySelector);
                else
                    query = query.OrderBy(orderByKeySelector);
            }

            if (includes?.Any() == true)
                foreach (var include in includes)
                    query = query.Include(include);

            int totalCount = await query.CountAsync();

            var items = await query.Skip((paging.PageNumber - 1) * paging.PageSize)
                                   .Take(paging.PageSize)
                                   .ToListAsync();

            return new PaginatedResponse<TEntity>()
            {
                TotalCount = totalCount,
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize,
                Data = items
            };
        }

    }
}
