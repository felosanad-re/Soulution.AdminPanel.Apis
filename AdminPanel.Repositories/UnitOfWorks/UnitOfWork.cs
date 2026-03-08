using AdminPanel.Core.Entities;
using AdminPanel.Core.GenericRepositories;
using AdminPanel.Core.UnitOfWord;
using AdminPanel.Repositories.Data;
using AdminPanel.Repositories.GenericRepositories;

namespace AdminPanel.Repositories.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdminDbContext _dbContext;
        private readonly Dictionary<Type, object> _repository = new ();
        public UnitOfWork(AdminDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<T> CreateRepository<T>() where T : ModelBase
        {
            var type = typeof(T);
            if(!_repository.ContainsKey(type))
            {
                _repository[type] = new GenericRepo<T>(_dbContext);
            }
            return  (IGenericRepository<T>)_repository[type];
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public ValueTask DisposeAsync()
            => _dbContext.DisposeAsync();
    }
}
