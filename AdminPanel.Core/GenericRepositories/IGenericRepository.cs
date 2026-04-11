using AdminPanel.Core.Entities;
using AdminPanel.Core.Specifications;
using System.Linq.Expressions;

namespace AdminPanel.Core.GenericRepositories
{
    public interface IGenericRepository<T> where T : ModelBase
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecifications<T> spec);
        Task<T?> GetAsyncSpec(ISpecifications<T> spec);
        Task<int> GetCountAsyncSpec(ISpecifications<T> spec);
        Task<IReadOnlyList<TResult>> GetSelectedAsync<TResult>(
                ISpecifications<T> spec,
                Expression<Func<T, TResult>> selector);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
    }
}
