using AutoMapper;
using IGift.Application.Models.Titulos;
using IGift.Application.Responses.Titulos.Categoria;

namespace IGift.Infrastructure.Mappings.Titulos
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<Categoria, CategoriaResponse>();
        }
    }
}
