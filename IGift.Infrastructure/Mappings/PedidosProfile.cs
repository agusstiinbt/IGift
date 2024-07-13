using AutoMapper;
using IGift.Application.Responses.Pedidos;
using IGift.Domain.Entities;

namespace IGift.Infrastructure.Mappings
{
    public class PedidosProfile : Profile
    {
        public PedidosProfile()
        {
            CreateMap<Pedidos, PedidosResponse>();
        }
    }
}
