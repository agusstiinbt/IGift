using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IGift.Application.Interfaces.Repositories.Generic.Auditable;
using IGift.Domain.Contracts;
using IGift.Infrastructure.Data;

namespace IGift.Infrastructure.Repositories.Generic.Auditable
{
    public class AuditableRepository<T, TId> : IAuditableRepository<T, TId> where T : AuditableEntity<TId>
    {
        private readonly ApplicationDbContext _context;

        public AuditableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> query => _context.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Task> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();//TODO fijarse si usar o no el cache remove que tiene blazorHero en esta clase en todos los metodos
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
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.FromResult(_context.Set<T>().Where(predicate).AsEnumerable());
        }
        public async Task<IQueryable<TDto>> FindAndMapByQuery<TDto>(IMapper mapper) where TDto : class
        {
            return await Task.FromResult(query.ProjectTo<TDto>(mapper.ConfigurationProvider));
        }

        public async Task UpdateAsync(T entity)
        {
            T exist = _context.Set<T>().Find(entity.Id)!;
            _context.Entry(exist).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

    }
}
