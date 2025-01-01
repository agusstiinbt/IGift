using AutoMapper;
using IGift.Application.Models;
using IGift.Application.Responses.Categoria;

namespace IGift.Infrastructure.Mappings
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<Categoria, CategoriaResponse>();
        }
    }
}
