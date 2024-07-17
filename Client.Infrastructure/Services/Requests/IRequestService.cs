using IGift.Application.Responses.Pedidos;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Requests
{
    public interface IPeticiones
    {
        Task<PaginatedResult<PeticionesResponse>> GetAll();
    }
}
