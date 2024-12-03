using System.Linq.Expressions;
using IGift.Domain.Contracts;

namespace IGift.Application.Filtros
{
    //TODO estudiar
    public interface ISpecification<T> where T : class, IEntity
    {
        /// <summary>
        /// Criterio de búsqueda
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }
        /// <summary>
        /// Lista de criterios de búsqueda que especifican qué entidades relacionadas deben incluirse en la consulta.
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }
        /// <summary>
        /// Lista de strings
        /// </summary>
        List<string> IncludeStrings { get; }
        Expression<Func<T, bool>> And(Expression<Func<T, bool>> query);
        Expression<Func<T, bool>> Or(Expression<Func<T, bool>> query);
    }
}
