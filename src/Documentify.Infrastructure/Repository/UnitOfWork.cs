using Documentify.ApplicationCore.Repository;
using Documentify.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentify.Infrastructure.Repository
{
    public class UnitOfWork<T, PK>(AppDbContext _context, IRepository<T, PK> repo) : IUnitOfWork<T, PK>
    {
        public IRepository<T, PK> Repository { get; } = repo;
        public async Task CompleteAsync(CancellationToken token = default)
        {
            await _context.SaveChangesAsync();
        }
    }
}
