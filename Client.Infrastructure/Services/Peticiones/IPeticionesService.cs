using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Peticiones
{
    public interface IPeticionesService
    {
        Task<IResult<PaginatedResult<PeticionesResponse>>> GetAll(GetAllPeticionesQuery request);
    }
}
