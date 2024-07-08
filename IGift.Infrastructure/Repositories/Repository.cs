using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Contracts;
using IGift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Repositories
{
    public class Repository<T, TId> : IRepository<T, TId> where T : AuditableEntity<TId>
    {
        private readonly ApplicationDbContext _context;

        public IQueryable<T> Entities => throw new NotImplementedException();

        public Task<T> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public Task<T> GetByIdAsync(TId id)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
