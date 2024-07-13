using AutoMapper;
using IGift.Application.Features.Pedidos.Command;
using IGift.Domain.Entities;

namespace IGift.Infrastructure.Mappings
{
    public class GiftProfile : Profile
    {

        public GiftProfile()
        {
            CreateMap<Pedidos, AddPedidoCommand>();
        }
    }
}
