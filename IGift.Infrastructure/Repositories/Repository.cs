using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Contracts;
using IGift.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IGift.Infrastructure.Repositories
{
    public class Repository<T, TId> : IRepository<T, TId> where T : AuditableEntity<TId>
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> Entities => _context.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();//TODO fijarse si usar o no el cache remove que tiene blazorHero
            return entity;
        }

        public async Task<Task> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();//TODO fijarse si usar o no el cache remove que tiene blazorHero
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(_context.Set<T>().AsEnumerable());
        }

        public async Task<T> GetByIdAsync(TId id)
        {
            return await _context!.Set<T>().FindAsync(id);
        }

        public Task<IEnumerable<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            //TODO utilizar?
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(T entity)
        {

            T exist = _context.Set<T>().Find(entity.Id);
            _context.Entry(exist).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }
    }
}
