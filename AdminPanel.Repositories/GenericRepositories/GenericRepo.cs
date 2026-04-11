using AdminPanel.Core.Entities;
using AdminPanel.Core.GenericRepositories;
using AdminPanel.Core.Specifications;
using AdminPanel.Repositories.Data;
using AdminPanel.Repositories.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdminPanel.Repositories.GenericRepositories
{
    public class GenericRepo<T> : IGenericRepository<T> where T : ModelBase
    {
        private readonly AdminDbContext _dbContext;

        public GenericRepo(AdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _dbContext.Set<T>().AsNoTracking().ToListAsync();

        public async Task<T?> GetAsync(int id)
            => await _dbContext.Set<T>().FindAsync(id);


        public async Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecifications<T> spec)
            => await AddSpecifications(spec).ToListAsync();

        public async Task<T?> GetAsyncSpec(ISpecifications<T> spec)
            => await AddSpecifications(spec).FirstOrDefaultAsync();

        public async Task<int> GetCountAsyncSpec(ISpecifications<T> spec)
            => await AddSpecifications(spec).CountAsync();

        // New Method To Get Select Column From Table
        public async Task<IReadOnlyList<TResult>> GetSelectedAsync<TResult>(
            ISpecifications<T> spec,
            Expression<Func<T, TResult>> selector)
        {
            var query = AddSpecifications(spec);

            return await query
                .AsNoTracking()
                .Select(selector)
                .ToListAsync();
        }

        public async Task AddAsync(T entity)
            => await _dbContext.Set<T>().AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbContext.AddRangeAsync(entities);

        public void Update(T entity)
            => _dbContext.Set<T>().Update(entity);

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);
        }

        private IQueryable<T> AddSpecifications(ISpecifications<T> spec)
        {
            return EvaluateSpec<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
