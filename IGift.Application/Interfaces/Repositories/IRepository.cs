﻿using IGift.Domain.Contracts;

namespace IGift.Application.Interfaces.Repositories
{

    public interface IRepository<T, in TId> where T : class, IEntity<TId>
    {
        IQueryable<T> Entities { get; }

        Task<T> GetByIdAsync(TId id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task<Task> DeleteAsync(T entity);
    }


    //    ¿Cuándo sería mejor usar directamente el DbContext?
    //Aunque el patrón repositorio es útil, hay casos en los que puedes optar por trabajar directamente con el DbContext:

    //Aplicaciones simples o prototipos:

    //Si la aplicación es pequeña y tiene pocos modelos, el overhead de crear repositorios puede no justificarse.
    //Operaciones complejas que no encajan bien en un repositorio:

    //Por ejemplo, si necesitas ejecutar consultas SQL muy específicas o manejar transacciones avanzadas.
    //Proyecto sin necesidad de test unitarios avanzados:

    //Si la testabilidad no es una prioridad, usar DbContext directamente puede ser más sencillo.
}
