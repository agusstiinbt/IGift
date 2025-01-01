using IGift.Application.Responses.Categoria;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Categoria
{
    public interface ICategoriaService
    {
        Task<IResult<IEnumerable<CategoriaResponse>>> GetAll();
    }
}
