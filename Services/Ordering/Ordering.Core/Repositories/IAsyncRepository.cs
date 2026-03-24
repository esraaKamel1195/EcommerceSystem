using Ordering.Core.Entities;
using System.Linq.Expressions;

namespace Ordering.Core.Repositories
{
    public interface IAsyncRepository<T> where T: EntityBase
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T Entity);
        Task UpdateAsync(T Entity);
        Task DeleteAsync(T Entity);
    }
}
