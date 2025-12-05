using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Entities.Common;
using Documentify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Documentify.Infrastructure.Repository
{
    public class Repository<T, TPK>(AppDbContext _context) : IRepository<T, TPK> where T : EntityBase
    {
        public async Task AddAsync(T entity, CancellationToken token = default)
        {
            await _context.Set<T>().AddAsync(entity, token);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public virtual async Task<bool> ExistsAsync(TPK id, CancellationToken token = default)
        {
            return await _context.Set<T>().AnyAsync(e => EF.Property<TPK>(e, "Id").Equals(id), token);
        }

        public async Task<T?> GetByIdAsync(TPK id, CancellationToken token = default)
        {
            return await _context.Set<T>().FindAsync(id, token);
        }

        public async Task<List<T>> ListAsync(CancellationToken token = default)
        {
            return await _context.Set<T>().ToListAsync(token);
        }

        public Task<List<T>> ListAsync(Specification<T> specification, CancellationToken token = default)
        {
            return _context.Set<T>().WithSpecification(specification).ToListAsync(token);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
