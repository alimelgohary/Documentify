using Ardalis.Specification;
namespace Documentify.ApplicationCore.Repository
{
    public interface IRepository<T, TPK>
    {
        Task<T?> GetByIdAsync(TPK id, CancellationToken token = default);
        Task<List<T>> ListAsync(CancellationToken token = default);
        Task<List<T>> ListAsync(Specification<T> specification, CancellationToken token = default);
        Task AddAsync(T entity, CancellationToken token = default);
        void DeleteAsync(T entity);
        void UpdateAsync(T entity);
        Task<bool> ExistsAsync(TPK id, CancellationToken token = default);
    }
}
