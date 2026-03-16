using AdminPanel.Core.Entities;
using AdminPanel.Core.GenericRepositories;

namespace AdminPanel.Core.UnitOfWork
{
    public interface IUnitOfWork: IAsyncDisposable
    {
        IGenericRepository<T> CreateRepository<T>() where T: ModelBase;
        Task<int> CompleteAsync();
    }
}
