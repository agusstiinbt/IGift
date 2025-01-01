using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Requests
{
    public interface IPeticionesService
    {
        Task<IResult<PaginatedResult<PeticionesResponse>>> GetAll(GetAllPeticionesQuery request);
    }
}
