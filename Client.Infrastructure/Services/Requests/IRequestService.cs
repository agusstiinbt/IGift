using IGift.Application.Features.Pedidos.Query;
using IGift.Application.Responses.Pedidos;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Requests
{
    public interface IPedidosService
    {
        Task<PaginatedResult<PedidosResponse>> GetAll();

    }
}
