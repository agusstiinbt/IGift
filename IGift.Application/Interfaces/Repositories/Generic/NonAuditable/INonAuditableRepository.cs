using System.Linq.Expressions;
using AutoMapper;
using IGift.Domain.Contracts;

namespace IGift.Application.Interfaces.Repositories
{
    public interface INonAuditableRepository<T, in TId> where T : class, IEntity<TId>
    {
        IQueryable<T> query { get; }

        Task<T> GetByIdAsync(TId id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Este metodo devuelve un query para que vayamos agregando Lambdas segun las propiedades que tenga la clase query
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="mapper"></param>
        /// <returns>EL query ya mapeado a la entidad correspondiente</returns>
        Task<IQueryable<TDto>> GetAllMapAsyncQuery<TDto>(IMapper mapper) where TDto : class;

        Task<Task> DeleteAsync(T entity);
    }
}
