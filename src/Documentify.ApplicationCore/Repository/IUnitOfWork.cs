using Documentify.Domain.Entities.Common;

namespace Documentify.ApplicationCore.Repository
{
    public interface IUnitOfWork
    {
        public IRepository<T, PK> Repository<T, PK>() where T : EntityBase;
        Task<int> CompleteAsync(CancellationToken token = default);
    }
}
