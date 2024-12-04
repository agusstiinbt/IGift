using AutoMapper;
using IGift.Application.CQRS.Peticiones.Command;
using IGift.Domain.Entities;

namespace IGift.Infrastructure.Mappings
{
    public class GiftProfile : Profile
    {

        public GiftProfile()
        {
            CreateMap<Peticiones, AddEditPeticionesCommand>();
        }
    }
}
