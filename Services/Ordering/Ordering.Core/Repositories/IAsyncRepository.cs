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
        Task<T> UpdateAsync(T Entity);
        Task<T> DeleteAsync(int id);
    }
}
