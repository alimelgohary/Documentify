using Documentify.ApplicationCore.Repository;
using Documentify.Domain.Entities.Common;
using Documentify.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Infrastructure.Repository
{
    public class UnitOfWork(AppDbContext _context) : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = new();
        public IRepository<T, PK> Repository<T, PK>() where T : EntityBase
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var repo = new Repository<T, PK>(_context);
                _repositories[type] = repo;
            }

            return (IRepository<T, PK>)_repositories[type];
        }
        public async Task<int> CompleteAsync(CancellationToken token = default)
        {
            return await _context.SaveChangesAsync();
        }
    }
}
