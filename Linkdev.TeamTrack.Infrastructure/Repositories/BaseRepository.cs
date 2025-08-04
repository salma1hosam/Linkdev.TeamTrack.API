using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Linkdev.TeamTrack.Infrastructure.Repositories
{
    public class BaseRepository<TEntity, TKey>(TeamTrackDbContext _dbContext) : IBaseRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);

        public void Remove(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

        public async Task<TEntity?> GetByIdAsync(TKey id) => await _dbContext.Set<TEntity>().FindAsync(id);
        
        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, params string[]? includes)
        {
            var items = _dbContext.Set<TEntity>().Where(predicate);
            if (includes.Any())
                foreach (var include in includes)
                    items = items.Include(include);

            return items;
        }

    }
}
