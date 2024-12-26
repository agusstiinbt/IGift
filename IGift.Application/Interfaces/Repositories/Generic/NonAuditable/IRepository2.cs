using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGift.Application.Interfaces.Repositories
{
	public interface IRepository2<T, in TId> where T : class, IEntity<TId>
	{
		IQueryable<T> Entities { get; }

		Task<T> GetByIdAsync(TId id);

		Task<IEnumerable<T>> GetAllAsync();

		Task<IEnumerable<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

		Task<T> AddAsync(T entity);

		Task UpdateAsync(T entity);

		Task<Task> DeleteAsync(T entity);
	}
}
