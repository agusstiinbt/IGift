using System.Linq.Expressions;
using IGift.Domain.Contracts;

namespace IGift.Application.Interfaces.Repositories
{
    public interface IRepository2<T, in TId> where T : class, IEntity<TId>
    {
        IQueryable<T> query { get; }

        Task<T> GetByIdAsync(TId id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<TDto>> FindByQuery<TDto>() where TDto : class;

        Task<Task> DeleteAsync(T entity);
    }
}
