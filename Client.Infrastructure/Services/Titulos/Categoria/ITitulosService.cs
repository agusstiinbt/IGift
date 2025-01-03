using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Titulos.Categoria
{
    public interface ITitulosService
    {
        public Task<IResult<BarraHerramientasConectadoResponse>> LoadConectado();
        public Task<IResult<BarraHerramientasDesconectadoResponse>> LoadDesconectado();
    }
}
