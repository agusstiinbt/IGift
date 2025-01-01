using AutoMapper;
using IGift.Application.Responses.Peticiones;
using IGift.Domain.Entities;

namespace IGift.Infrastructure.Mappings
{
    public class PeticionesProfile : Profile
    {
        public PeticionesProfile()
        {
            CreateMap<Peticiones, PeticionesResponse>();
        }
    }
}
