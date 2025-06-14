using AutoMapper;
using IGift.Application.Models.SQL.MySQL;
using IGift.Application.Responses.Titulos.Categoria;

namespace IGift.Infrastructure.Mappings.Titulos
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<Category, CategoriaResponse>();
        }
    }
}
