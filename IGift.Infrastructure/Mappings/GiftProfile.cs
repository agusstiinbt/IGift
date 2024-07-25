using AutoMapper;
using IGift.Application.Requests.Peticiones.Pedidos.Command;
using IGift.Domain.Entities;

namespace IGift.Infrastructure.Mappings
{
    public class GiftProfile : Profile
    {

        public GiftProfile()
        {
            CreateMap<Peticiones, AddPeticionesCommand>();
        }
    }
}
