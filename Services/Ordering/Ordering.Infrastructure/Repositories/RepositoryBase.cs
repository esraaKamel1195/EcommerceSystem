using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : EntityBase
    {
        public readonly OrderContext _orderContext;
        public RepositoryBase (OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _orderContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _orderContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _orderContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T Entity)
        {
            _orderContext.Set<T>().Add(Entity);
            await _orderContext.SaveChangesAsync();
            return Entity;
        }

        public async Task UpdateAsync(T Entity)
        {
            _orderContext.Entry(Entity).State = EntityState.Modified;
            await _orderContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T Entity)
        {
            _orderContext.Set<T>().Remove(Entity);
            await _orderContext.SaveChangesAsync();
        }
    }
}
