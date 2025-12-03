namespace Documentify.ApplicationCore.Repository
{
    public interface IUnitOfWork<T, PK>
    {
        public IRepository<T, PK> Repository { get; }
        Task CompleteAsync(CancellationToken token = default);
    }
}
