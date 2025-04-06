using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Peticiones
{
    public interface IPeticionesService
    {
        /// <summary>
        /// Hace un llamado al servidor enviando una clase de tipo Query para devolvernos un response paginado con las peticiones creadas
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Un response compaginado de las peticiones creadas</returns>
        Task<PaginatedResult<PeticionesResponse>> GetAll(GetAllPeticionesQuery request);
    }
}
