using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Contracts;
using IGift.Infrastructure.Data;

namespace IGift.Infrastructure.Repositories
{
    public class Repository2<T, TId> : IRepository2<T, TId> where T : Entity<TId>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Repository2(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        public async Task<IQueryable<TDto>> FindByQuery<TDto>() where TDto : class
        {
            return query.ProjectTo<TDto>(_mapper.ConfigurationProvider);
        }

        public async Task UpdateAsync(T entity)
        {
            T exist = _context.Set<T>().Find(entity.Id)!;
            _context.Entry(exist).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }
    }

}
