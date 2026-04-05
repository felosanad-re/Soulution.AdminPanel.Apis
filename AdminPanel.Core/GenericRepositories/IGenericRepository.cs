using AdminPanel.Core.Entities;
using AdminPanel.Core.Specifications;

namespace AdminPanel.Core.GenericRepositories
{
    public interface IGenericRepository<T> where T : ModelBase
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecifications<T> spec);
        Task<T?> GetAsyncSpec(ISpecifications<T> spec);
        Task<int> GetCountAsyncSpec(ISpecifications<T> spec);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
