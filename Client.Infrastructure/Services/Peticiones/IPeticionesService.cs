using IGift.Application.Features.Pedidos.Query;
using IGift.Application.Responses.Pedidos;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Requests
{
    public interface IPeticionesService
    {
        Task<IResult<PaginatedResult<PeticionesResponse>>> GetAll(GetAllPeticionesQuery request);
    }
}
