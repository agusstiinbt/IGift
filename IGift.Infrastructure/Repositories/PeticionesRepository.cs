using IGift.Application.Interfaces.Repositories;
using IGift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace IGift.Infrastructure.Repositories
{
    public class PeticionesRepository : IPeticionesRepository
    {
        private readonly IRepository<Peticiones, string> _repository;

        public PeticionesRepository(IRepository<Peticiones, string> repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsBrandUsed(string id)
        {
            return await _repository.Entities.AsNoTracking().AnyAsync(x => x.Id == id);
        }
    }
}

//    ¿Cuándo sería mejor usar directamente el DbContext?
//Aunque el patrón repositorio es útil, hay casos en los que puedes optar por trabajar directamente con el DbContext:

//Aplicaciones simples o prototipos:

//Si la aplicación es pequeña y tiene pocos modelos, el overhead de crear repositorios puede no justificarse.
//Operaciones complejas que no encajan bien en un repositorio:

//Por ejemplo, si necesitas ejecutar consultas SQL muy específicas o manejar transacciones avanzadas.
//Proyecto sin necesidad de test unitarios avanzados:

//Si la testabilidad no es una prioridad, usar DbContext directamente puede ser más sencillo.


//    Cuando usas AnyAsync directamente en el repositorio, generas una consulta SQL que evalúa solo si existe al menos un registro que cumpla con la condición.Esto es mucho más eficiente que traer un registro completo con GetByIdAsync para luego hacer el filtrado en memoria.
