namespace Documentify.ApplicationCore.Repository
{
    public interface IUnitOfWork
    {
        public IRepository<T, PK> Repository<T, PK>() where T : class;
        Task<int> CompleteAsync(CancellationToken token = default);
    }
}
